namespace Ibdal.Models.Abstractions;

public abstract class AppUserModel : Archivable
{
    public required string AuthId { get; set; }
    public int Points { get; set; } = 0;
    public required string PhoneNumber { get; set; }
    public IList<Car> Cars { get; set; } = [];
    public IList<Message> Messages { get; set; } = [];
}