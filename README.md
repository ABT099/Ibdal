# Ibdal Backend

Ibdal is a backend API developed using C# and ASP.NET 8 to support a mobile application for customers and station users, as well as a web application for administrators. The system manages vehicle services such as repairs and oil changes, communication between customers and stations, and order management.

This backend API is built on top of the open-source library [Identity.Mongo](https://github.com/ABT099/Identity.Mongo) for handling user authentication and authorization with MongoDB.

## Features

### Core Functionalities
1. **User Management:**
   - Account creation, login, and profile management.
   - Vehicle management, allowing customers to add and manage their vehicles.

2. **Service Management:**
   - Stations can provide services such as repairs, oil changes, and other maintenance tasks.
   - Product sales and order placement by stations.

3. **Communication & Notifications:**
   - Real-time chat between customers and stations.
   - Notification system for service updates, promotions, and relevant information.

### Roles
- **Customer (Mobile App):** Creates an account, manages vehicles, and requests services.
- **Station (Mobile App):** Provides services, manages product sales, and places orders.
- **Admin (Web App):** Manages users, stations, and overall system settings.

## Technology Stack

- **Backend Language**: C# (.NET 8)
- **Database**: MongoDB
- **Authentication**: [Identity.Mongo](https://github.com/ABT099/Identity.Mongo)
- **API Documentation**: Swagger UI

## Setup Instructions

### Prerequisites
- .NET SDK 8
- MongoDB (local or cloud instance)

### Configuration

Update your MongoDB settings in `appsettings.json`:

```json
"MongoDb": {
  "ConnectionString": "your_mongodb_connection_string",
  "DatabaseName": "your_database_name"
}
```

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd ibdal-backend
   ```

2. **Restore Packages**
   ```bash
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

The API will be accessible locally at `https://localhost:<port>/api`.

### Swagger Documentation

Swagger UI is enabled for this API, providing an interactive interface for testing all endpoints. After running the application, navigate to:

```
https://localhost:<port>/swagger/index.html
```

This will display all available endpoints and their details.

## Built With Identity.Mongo

The [Identity.Mongo](https://github.com/ABT099/Identity.Mongo) library provides the authentication and authorization functionality of this application, enabling efficient user and role management with MongoDB.

## Contributing

This repository is intended for portfolio display, and external contributions are not currently open. However, feedback is welcome.

## License

This project is for private use only and is not intended for commercial distribution.
