microblogging application built with .NET 8, featuring JWT authentication, image processing, and Azure-ready deployment.
## Local Development

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or use Docker)
- [Azure Storage Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) (for local storage)

### Running the Application
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/microblogging.git
   cd microblogging
2. Configure app settings:
cp Microblogging.Web/appsettings.Development.json.example Microblogging.Web/appsettings.Development.json
Edit the file with your local credentials.

3. Run the application:
dotnet run --project Microblogging.Web
The app will be available at https://localhost:5001

4. Database Setup
SQL Server
sqllocaldb create "MicrobloggingDB"
sqllocaldb start "MicrobloggingDB"
#Migrations
Apply migrations:
dotnet ef database update --project Microblogging.Data
Generate new migrations:
dotnet ef migrations add InitialCreate --project Microblogging.Data


