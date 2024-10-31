using Ibdal.Api.Data;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PurchasesController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchases = await ctx.Purchases
            .Find(_ => true)
            .ToListAsync();
        
        return Ok(purchases);
    }

    [HttpGet("station/{stationId}")]
    public async Task<IActionResult> GetByStationId(string stationId)
    {
        var purchases = await ctx.Purchases
            .Find(x => x.Order.StationId == stationId)
            .Project(PurchaseViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(purchases);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var purchases = await ctx.Purchases
            .Find(x => x.Id == id)
            .Project(PurchaseViewModels.Projection)
            .FirstOrDefaultAsync();

        if (purchases == null)
        {
            return NotFound();
        }
        
        return Ok(purchases);
    }

    [HttpPost("{id}/sell")]
    public async Task<IActionResult> Sell([FromRoute] string id, [FromBody] int quantity)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var quantityRemaining = await ctx.Purchases
                .Find(x => x.Id == id)
                .Project(x => x.QuantityRemaining)
                .FirstOrDefaultAsync();

            if (quantityRemaining < quantity || quantityRemaining == 0)
            {
                return BadRequest("Quantity isn't sufficient");
            }
            
            quantityRemaining -= quantity;
            
            await ctx.Purchases.UpdateOneAsync(
                session, 
                x => x.Id == id,
                Builders<Purchase>.Update.Set(x => x.QuantityRemaining, quantityRemaining));
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("Something went wrong");
        }
    }

    [HttpPost("{id}/add-payment")]
    public async Task<IActionResult> AddPayment([FromRoute] string id, [FromBody] int paymentAmount)
    {
        var info = await ctx.Purchases
            .Find(x => x.Id == id)
            .Project(x => new
            {
                TotalPrice = x.Order.ProductsInfo.Select(y => y.Product.Price * y.Quantity).Sum(),
                TotalPayments = x.Payments.Select(z => z.Amount).Sum()
            })
            .FirstOrDefaultAsync();

        if (info is null)
        {
            return NotFound();
        }

        if (info.TotalPrice <= info.TotalPayments ||
            paymentAmount > info.TotalPrice ||
            paymentAmount <= 0)
        {
            return BadRequest();
        }
        
        var payment = new Payment
        {
            Amount = paymentAmount,
            PurchaseId = id
        };
        
        await ctx.Purchases.UpdateOneAsync(
            x => x.Id == id,
            Builders<Purchase>.Update.Push(x => x.Payments, payment));
        
        return Ok();
    }
}