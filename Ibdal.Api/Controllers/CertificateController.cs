using Ibdal.Api.Data;
using Ibdal.Api.Services;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CertificateController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var certificates = await ctx.Certificates
            .Find(_ => true)
            .Project(CertificateViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(certificates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var certificate = await ctx.Certificates
            .Find(x => x.Id == id)
            .Project(CertificateViewModels.Projection)
            .FirstOrDefaultAsync();

        if (certificate == null)
        {
            return NotFound();
        }
        
        return Ok(certificate);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCertificateForm createCertificateForm)
    {
        if (createCertificateForm.Images.Count() < 0)
            return BadRequest();

        var urls = createCertificateForm.Images.Select(FileService.SaveFile).ToList();

        var certificate = new Certificate
        {
            Name = createCertificateForm.Name,
            Description = createCertificateForm.Description,
            ImagesUrls = urls
        };

        await ctx.Certificates.InsertOneAsync(certificate);
        
        return CreatedAtAction(nameof(GetById), new { id = certificate.Id }, certificate.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCertificateForm updateCertificateForm)
    {
        var certificate = await ctx.Certificates
            .Find(x => x.Id == updateCertificateForm.Id)
            .FirstOrDefaultAsync();

        if (certificate == null)
        {
            return NotFound();
        }
        
        certificate.Name = updateCertificateForm.Name;
        certificate.Description = updateCertificateForm.Description;
        
        await ctx.Certificates.ReplaceOneAsync(x => x.Id == certificate.Id, certificate);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await ctx.Certificates.DeleteOneAsync(x => x.Id == id);

        if (!result.IsAcknowledged)
        {
            return NotFound();
        }
        
        return Ok();
    }
    
}