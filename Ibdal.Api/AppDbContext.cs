using Ibdal.Models;
using Identity.Mongo;
using MongoDB.Driver;

namespace Ibdal.Api;

public class AppDbContext : MongoDbContext
{
    protected override void ConfigureCollections()
    {
        Cars = Database.GetCollection<Car>("cars");
        Categories = Database.GetCollection<Category>("categories");
        CategoryItems = Database.GetCollection<CategoryItem>("category_items");
        Certificates = Database.GetCollection<Certificate>("certificates");
        Content = Database.GetCollection<Content>("content");
        Messages = Database.GetCollection<Message>("messages");
        Notifications = Database.GetCollection<Notification>("notifications");
        OilChanges = Database.GetCollection<OilChange>("oil_changes");
        Orders = Database.GetCollection<Order>("orders");
        Payments = Database.GetCollection<Payment>("payments");
        Products = Database.GetCollection<Product>("products");
        Purchases = Database.GetCollection<Purchase>("purchases");
        Repairs = Database.GetCollection<Repair>("repairs");
        Stations = Database.GetCollection<Station>("stations");
        Users = Database.GetCollection<User>("users");
    }

    public required IMongoCollection<Car> Cars { get; set; }
    public required IMongoCollection<Category> Categories { get; set; }
    public required IMongoCollection<CategoryItem> CategoryItems { get; set; }
    public required IMongoCollection<Certificate> Certificates { get; set; }
    public required IMongoCollection<Content> Content { get; set; }
    public required IMongoCollection<Message> Messages { get; set; }
    public required IMongoCollection<Notification> Notifications { get; set; }
    public required IMongoCollection<OilChange> OilChanges { get; set; }
    public required IMongoCollection<Order> Orders { get; set; }
    public required IMongoCollection<Payment> Payments { get; set; }
    public required IMongoCollection<Product> Products { get; set; }
    public required IMongoCollection<Purchase> Purchases { get; set; }
    public required IMongoCollection<Repair> Repairs { get; set; }
    public required IMongoCollection<Station> Stations { get; set; }
    public required IMongoCollection<User> Users { get; set; }
}