using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class User : AppUserModel
{
    public required string Name { get; set; }
}