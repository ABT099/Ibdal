namespace Ibdal.Models.Abstractions;

public abstract class AppUserModel : Archivable
{
    protected AppUserModel()
    {
        Chat = new Chat
        {
            UserId = Id
        };
    }

    public required string AuthId { get; set; }
    public int Points { get; set; } = 0;
    public required string PhoneNumber { get; set; }
    public IList<Car> Cars { get; set; } = [];
    public Chat Chat { get; set; }
}