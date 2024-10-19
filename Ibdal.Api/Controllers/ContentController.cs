namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContentController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var content = await ctx.Content
            .Find(_ => true)
            .Project(ContentViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(content);
    }
    
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(string type)
    {
        var content = await ctx.Content
            .Find(x => x.Type == type)
            .Project(ContentViewModels.Projection)
            .ToListAsync();

        if (content == null)
        {
            return NotFound();
        }
        
        return Ok(content);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContentForm createContentForm)
    {
        var content = new Content
        {
            Text = createContentForm.Text,
            Type = createContentForm.Type
        };

        await ctx.Content.InsertOneAsync(content);
        
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateContentForm updateContentForm)
    {
        var content = await ctx.Content
            .Find(x => x.Id == updateContentForm.Id)
            .FirstOrDefaultAsync();

        if (content == null)
        {
            return NotFound();
        }
        
        content.Text = updateContentForm.Text;
        content.Type = updateContentForm.Type;
        
        await ctx.Content.ReplaceOneAsync(x => x.Id == updateContentForm.Id, content);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await ctx.Content.DeleteOneAsync(x => x.Id == id);

        if (!result.IsAcknowledged)
        {
            return NotFound();
        }
        
        return Ok();
    }
}