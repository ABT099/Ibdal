using MongoDB.Bson;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await ctx.Orders
            .Find(_ => true)
            .Project(OrderViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(orders);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await ctx.Orders
            .Find(x => x.Id == id)
            .Project(OrderViewModels.Projection)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }
    
    [HttpGet("{id}/admin")]
    public async Task<IActionResult> GetByIdForAdmins(string id)
    {
        var order = await ctx.Orders
            .Find(x => x.Id == id)
            .Project(OrderViewModels.AdminProjection)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderForm createOrderForm)
    {
        var productIds = createOrderForm.Products.Select(x => x.ProductId).ToHashSet();
        
        var productsTask = ctx.Products
            .Find(x => productIds.Contains(x.Id))
            .ToListAsync();

        var stationTask = ctx.Stations
            .Find(x => x.Id == createOrderForm.StationId)
            .Project(station => new 
            {
                station.Name,
            })
            .FirstOrDefaultAsync();
        
        await Task.WhenAll(productsTask, stationTask);

        var products = await productsTask;
        var station = await stationTask;

        if (products.Count == 0)
        {
            return NotFound("No products found.");
        }

        if (station == null)
        {
            return NotFound("No station found.");
        }

        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var order = new Order
            {
                OrderNumber = 0,
                StationId = createOrderForm.StationId,
                StationName = station.Name,
                ProductsInfo = createOrderForm.Products.Select(p => new ProductOrder
                    {
                        Product = products.First(prod => prod.Id == p.ProductId),
                        Quantity = p.Quantity
                    })
                    .ToList(),
                PurchaseId = string.Empty
            };

            var purchase = new Purchase
            {
                Order = order,
                QuantityRemaining = order.ProductsInfo.Sum(x => x.Quantity)
            };
            
            var insertOrderTask = ctx.Orders.InsertOneAsync(session, order);
            var insertPurchaseTask = ctx.Purchases.InsertOneAsync(session, purchase);
            
            await Task.WhenAll(insertOrderTask, insertPurchaseTask);

            order.PurchaseId = purchase.Id;
            
            var orderUpdateTask = ctx.Orders
                .UpdateOneAsync(
                    session,
                    x => x.Id == order.Id,
                    Builders<Order>.Update.Set(x => x.PurchaseId, purchase.Id));
            
            var purchaseUpdateTask = ctx.Purchases.UpdateOneAsync(
                session,
                x=>x.Id == purchase.Id,
                Builders<Purchase>.Update.Set(x => x.Order, order));
            
            await Task.WhenAll(orderUpdateTask, purchaseUpdateTask);
            
            purchase.Order = order;
            
            var stationUpdateResult = await ctx.Stations
                .UpdateOneAsync(
                    session,
                    x => x.Id == createOrderForm.StationId,
                    Builders<Station>.Update.Push(x => x.Purchases, purchase));
          
            if (!stationUpdateResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return Problem("An error occured while updating the station.");
            }
            
            await session.CommitTransactionAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.OrderNumber }, order);
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while creating the order.");
        }
    }

    [HttpPatch("{orderId}")]
    public async Task<IActionResult> UpdateStatus([FromRoute] string orderId, [FromBody] string status)
    {
        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
        {
            return BadRequest();
        }
        
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var stationId = await ctx.Orders
                .Find(x => x.Id == orderId)
                .Project(x => x.StationId)
                .FirstOrDefaultAsync();
            
            var statusUpdateTask = ctx.Orders
                .UpdateOneAsync(
                    session,
                    x => x.Id == orderId,
                    Builders<Order>.Update.Set(x => x.Status, status));
            
            
            var arrayFilter = new BsonDocument("elem.Id", orderId);
            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(arrayFilter)
            };

            var stationOrdersUpdateTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == stationId,
                Builders<Station>.Update.Set("Orders.$[elem].Status", status),
                new UpdateOptions
                {
                    ArrayFilters = arrayFilters
                });

            await Task.WhenAll(statusUpdateTask, stationOrdersUpdateTask);
            
            var statusResult = await statusUpdateTask;
            var stationResult = await stationOrdersUpdateTask;

            if (!statusResult.IsAcknowledged || !stationResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return Problem("An error occured while updating the order.");
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while updating the order.");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var info = await ctx.Orders
                .Find(session, x => x.Id == id)
                .Project(x => new
                {
                    x.StationId,
                    x.PurchaseId
                })
                .FirstOrDefaultAsync();

            if (info == null)
            {
                await session.AbortTransactionAsync();
                return BadRequest("No station found.");
            }
            
            var removeOrderResult = await ctx.Orders.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Order>.Update.Set(x => x.IsDeleted, true));

            var stationUpdateResult = await ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == info.StationId,
                Builders<Station>.Update.PullFilter(x => x.Purchases, p => p.Id == info.PurchaseId));

            if (!stationUpdateResult.IsAcknowledged || !removeOrderResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound();
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while deleting the order.");
        }
    }
}