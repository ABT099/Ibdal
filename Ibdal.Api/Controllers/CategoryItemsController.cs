using Ibdal.Api.Data;
using Ibdal.Api.Services;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryItemsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetAllByCategoryId(string categoryId)
    {
        var cItems = await ctx.CategoryItems
            .Find(x => x.CategoryId == categoryId)
            .Project(CategoryItemViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(cItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var cItem = await ctx.CategoryItems
            .Find(x => x.Id == id)
            .Project(CategoryItemViewModels.Projection)
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
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var imgUrl = FileService.SaveFile(createCategoryItemForm.Image);
            
            var categoryItem = new CategoryItem
            {
                CategoryId = createCategoryItemForm.CategoryId,
                Name = createCategoryItemForm.Name,
                Description = createCategoryItemForm.Description,
                ImageUrl = imgUrl
            };
            await ctx.CategoryItems.InsertOneAsync(session, categoryItem);
            
            var categoryResult = await ctx.Categories.UpdateOneAsync(
                session, 
                x => x.Id == createCategoryItemForm.CategoryId, 
                Builders<Category>.Update.Push(c => c.CategoryItems, categoryItem));

            if (!categoryResult.IsAcknowledged)
            {
                FileService.DeleteFile(imgUrl);
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
            imgUrl = FileService.SaveFile(updateCategoryItemForm.Image);
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
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var result = await ctx.CategoryItems.DeleteOneAsync(session, x => x.Id == id);

            if (!result.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("category item not found");
            }
            
            var info = await ctx.CategoryItems.Find(x => x.Id == id)
                .Project(x => new
                {
                    x.CategoryId,
                    x.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (info == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("category item not found");
            }

            await ctx.Categories.UpdateOneAsync(
                session,
                x => x.Id == info.CategoryId,
                Builders<Category>.Update.PullFilter(x => x.CategoryItems, x => x.Id == id));

            FileService.DeleteFile(info.ImageUrl);

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
}