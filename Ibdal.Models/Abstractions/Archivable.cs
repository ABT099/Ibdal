namespace Ibdal.Models.Abstractions;

public class Archivable<TKey> : BaseModel<TKey>
{
    public bool IsDeleted { get; set; } = false;
}