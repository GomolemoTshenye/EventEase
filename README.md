# EventEase 🎉

**EventEase** is a modern web application for managing events, venues, and bookings. Built with ASP.NET Core and Entity Framework Core, it provides a clean and efficient way to organize events, reserve venues, and handle bookings.

## ✨ Features

- **Venue Management**: Add and manage event venues with details like capacity, location, and images (stored in Azure Blob Storage / Azurite).
- **Event Management**: Create events with start/end dates, descriptions, and associate them with venues.
- **Booking System**: Book venues for specific events with proper validation and tracking.
- **Image Upload**: Seamless upload of venue images using Azure Blob Storage.
- **Data Validation**: Robust validation using Data Annotations.
- **Database Integration**: Built with Entity Framework Core for easy data persistence.

## 🛠 Tech Stack

- **Backend**: ASP.NET Core Web API / MVC
- **Database**: SQL Server (with EF Core)
- **Storage**: Azure Blob Storage (Azurite emulator for local development)
- **Frontend**: Razor Pages / Blazor / React (depending on your implementation)
- **ORM**: Entity Framework Core
- **Validation**: DataAnnotations

## 📁 Project Structure
EventEase/
├── EventEase.Models/          # Entity models (Venue, Event, Booking)
├── EventEase.Services/        # Business logic and services (BlobService, etc.)
├── EventEase.Web/             # Web application (Controllers, Views, etc.)
├── EventEase.Data/            # DbContext and migrations
└── README.md
text## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server (LocalDB or full instance)
- Azurite (Azure Storage Emulator) for local blob storage

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/EventEase.git
   cd EventEase

Configure Connection Strings
Update appsettings.json with your database and Azure Storage connection strings:JSON{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventEase;Trusted_Connection=True;",
    "AzureStorage": "UseDevelopmentStorage=true"
  }
}
Run Azurite (for local blob storage)Bashazurite --silent --location ./azurite
Apply Database MigrationsBashdotnet ef database update
Run the ApplicationBashdotnet run


🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.
📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

Made with ❤️ for event organizers everywhere.
