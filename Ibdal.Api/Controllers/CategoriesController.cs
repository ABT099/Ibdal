namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await ctx.Categories
            .Find(_ => true)
            .Project(CategoryViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var category = await ctx.Categories
            .Find(x => x.Id == id)
            .Project(CategoryViewModels.Projection)
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }
        
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryForm createCategoryForm)
    {
        var category = new Category
        {
            Name = createCategoryForm.Name,
        };

        await ctx.Categories.InsertOneAsync(category);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryForm updateCategoryForm)
    {
        var category = await ctx.Categories
            .Find(x => x.Id == updateCategoryForm.Id)
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }
        
        category.Name = updateCategoryForm.Name;
        
        await ctx.Categories.ReplaceOneAsync(x => x.Id == updateCategoryForm.Id, category);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var category = await ctx.Categories
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }
        
        await ctx.Categories.DeleteOneAsync(x => x.Id == id);
        
        return Ok();
    }
}