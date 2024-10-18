namespace Ibdal.Models.Abstractions;

public class Archivable : BaseModel
{
    public bool IsDeleted { get; set; } = false;
}