namespace Ibdal.Api.Forms;

public class CreateCertificateForm
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IEnumerable<IFormFile> Images { get; set; } = [];
}