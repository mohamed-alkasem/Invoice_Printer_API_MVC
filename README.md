# Invoice Printer API & MVC

A robust solution for managing and printing invoices, built with **.NET 9.0 (ASP.NET Core)**. This project combines a flexible API architecture with an MVC frontend to provide a seamless invoicing experience.

## Architecture
This project is built using a modern **Clean Architecture** approach to ensure scalability and ease of maintenance.

- **Framework:** .NET 9.0 (ASP.NET Core)
- **Pattern:** MVC (Model-View-Controller) + API
- **Data Access:** Entity Framework Core
- **Database:** Microsoft SQL Server

## Key Features
- **API-First Design:** Exposes clean endpoints for invoice management.
- **MVC Frontend:** A user-friendly interface for managing and triggering invoice printing.
- **Print Integration:** Optimized for document generation and printing workflows.
- **Scalable Structure:** Clean separation between the Web/UI layer, Application logic, and Data infrastructure.

## Getting Started

### Prerequisites
- .NET 9.0 SDK or higher
- SQL Server

### Configuration
1. **Clone the repository:**
   git clone [https://github.com/mohamed-alkasem/Invoice_Printer_API_MVC.git](https://github.com/mohamed-alkasem/Invoice_Printer_API_MVC.git)
Environment Setup:

Create an appsettings.json file in the root directory.

Configure your SQL Server ConnectionStrings to point to your local database.

Note: Ensure appsettings.json is kept local and is NOT committed to the repository to maintain security.

Database Initialization
Apply the necessary migrations to your local SQL Server instance:

dotnet ef database update
Running the Application
Launch the project using the .NET CLI:

dotnet run
Once started, the application will be available at https://localhost:7xxx.

API Documentation
The project includes built-in Swagger UI for testing and documentation of all available endpoints. You can access it via /swagger when the application is running.

Project Structure
/Invoice_Printer: Main project containing the source code.

/Controllers: Handles incoming requests and orchestrates the flow between the View and the API logic.

/Models: Defines the data structure for invoices and associated entities.

/Data: Manages database contexts and migration files.

Status
Development: Active.

Frontend: Focused on MVC implementation and print-friendly views.

Backend: Core API features are functional and ready for extension.

Developed by Mohamad Alkassem
