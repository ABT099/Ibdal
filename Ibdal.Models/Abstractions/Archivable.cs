namespace Ibdal.Models.Abstractions;

public class Archivable : BaseModel
{
    public bool Archived { get; set; } = false;
}