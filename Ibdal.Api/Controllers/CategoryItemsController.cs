using Ibdal.Api.Services;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryItemsController(AppDbContext ctx, FileService fileService) : ControllerBase
{
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetAllByCategoryId(string categoryId)
    {
        var cItems = await ctx.CategoryItems
            .Find(x => x.CategoryId == categoryId)
            .ToListAsync();
        
        return Ok(cItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var cItem = await ctx.CategoryItems
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (cItem == null)
        {
            return NotFound();
        }
        
        return Ok(cItem);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryItemForm createCategoryItemForm)
    {
        var category = await ctx.Categories
            .Find(x => x.Id == createCategoryItemForm.CategoryId)
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound("category not found");
        }
        
        var imgUrl = fileService.SaveImage(createCategoryItemForm.Image);

        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var categoryItem = new CategoryItem
            {
                CategoryId = createCategoryItemForm.CategoryId,
                Name = createCategoryItemForm.Name,
                Description = createCategoryItemForm.Description,
                ImageUrl = imgUrl
            };
            await ctx.CategoryItems.InsertOneAsync(session, categoryItem);
            
            await ctx.Categories.UpdateOneAsync(
                session, 
                x => x.Id == createCategoryItemForm.CategoryId, 
                Builders<Category>.Update.Push(c => c.CategoryItems, categoryItem)); // Example of updating a field

            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("something went wrong.");
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryItemForm updateCategoryItemForm)
    {
        var categoryItem = await ctx.CategoryItems
            .Find(x => x.Id == updateCategoryItemForm.Id)
            .FirstOrDefaultAsync();

        if (categoryItem == null)
        {
            return NotFound("category not found");
        }
        
        var category = await ctx.Categories
            .Find(x => x.Id == updateCategoryItemForm.CategoryId)
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound("category not found");
        } 
        
        var imgUrl = string.Empty;
        
        if (updateCategoryItemForm.Image != null)
        {
            imgUrl = fileService.SaveImage(updateCategoryItemForm.Image);
        }
        
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            categoryItem.Name = updateCategoryItemForm.Name;
            categoryItem.Description = updateCategoryItemForm.Description;
            if (!string.IsNullOrWhiteSpace(imgUrl))
            {
                categoryItem.ImageUrl = imgUrl;
            }

            await ctx.CategoryItems.ReplaceOneAsync(session, x => x.Id == updateCategoryItemForm.Id, categoryItem);

            var cItemIndex = category.CategoryItems.IndexOf(categoryItem);

            if (cItemIndex != -1)
            {
                category.CategoryItems[cItemIndex] = categoryItem;
            }
            else
            {
                category.CategoryItems.Add(categoryItem);
            }

            await ctx.Categories.ReplaceOneAsync(session, x => x.Id == updateCategoryItemForm.CategoryId, category);
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("something went wrong.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var categoryItem = await ctx.CategoryItems
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (categoryItem == null)
        {
            return NotFound();
        }
        
        await ctx.CategoryItems.DeleteOneAsync(x => x.Id == id);
        
        return Ok();
    }
}