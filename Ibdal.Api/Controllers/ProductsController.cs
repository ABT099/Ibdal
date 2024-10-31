using Ibdal.Api.Data;
using Ibdal.Api.Services;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]  
public class ProductsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await ctx.Products
            .FindNonArchived(_ => true)
            .Project(ProductViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await ctx.Products
            .FindNonArchived(x => x.Id == id)
            .Project(ProductViewModels.Projection)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductForm createProductForm)
    {
        var imageUrl = FileService.SaveFile(createProductForm.Image);
        
        var product = new Product
        {
            Name = createProductForm.Name,
            Description = createProductForm.Description,
            Category = createProductForm.Category,
            Price = createProductForm.Price,
            ImageUrl = imageUrl
        };

        await ctx.Products.InsertOneAsync(product);
        
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProductForm updateProductForm)
    {
        var product = await ctx.Products
            .FindNonArchived(x => x.Id == updateProductForm.Id)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound();
        }
        
        product.Name = updateProductForm.Name;
        product.Description = updateProductForm.Description;
        product.Category = updateProductForm.Category;
        product.Price = updateProductForm.Price;
        
        if (updateProductForm.Image is not null)
        {
            var imageUrl = FileService.SaveFile(updateProductForm.Image);
            product.ImageUrl = imageUrl;
        }
        
        await ctx.Products.ReplaceOneAsync(x => x.Id == product.Id, product);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await ctx.Products.UpdateOneAsync(
            x => x.Id == id,
            Builders<Product>.Update.Set(x => x.Archived, true));

        if (!result.IsAcknowledged)
        {
            return NotFound();
        }
        
        return Ok();
    }
}