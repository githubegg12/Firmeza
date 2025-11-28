# Firmeza - Sales Management System

A comprehensive sales management system built with ASP.NET Core, Angular, and PostgreSQL. The solution follows Clean Architecture principles and includes separate applications for API, Admin Panel, and Client Portal.

## 🏗️ Architecture

This solution implements Clean Architecture with the following layers:

```
Firmeza/
├── Firmeza.Domain/          # Core business entities and interfaces
├── Firmeza.Application/     # Business logic and use cases
├── Firmeza.Infrastructure/  # Data access and external services
├── Firmeza.Identity/        # Authentication and authorization
├── Firmeza.API/            # RESTful API (ASP.NET Core)
├── Firmeza.web/            # Admin Panel (ASP.NET Core MVC)
├── Firmeza.Client/         # Client Portal (Angular)
└── Firmeza.Tests/          # Unit and integration tests
```

## 🚀 Features

- **Product Management**: CRUD operations for products with categories, pricing, and inventory
- **Sales Processing**: Complete sales workflow with PDF receipt generation
- **User Management**: Role-based access control (Admin/Client)
- **Bulk Import**: Excel file import for products and clients
- **PDF Generation**: Automatic receipt generation for sales
- **Email Notifications**: Welcome emails and notifications
- **RESTful API**: JWT-based authentication
- **Admin Dashboard**: Comprehensive admin panel with metrics
- **Client Portal**: Angular-based customer interface

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL 15
- **Authentication**: ASP.NET Core Identity + JWT
- **PDF Generation**: QuestPDF
- **Excel Processing**: EPPlus
- **Email**: SMTP

### Frontend
- **Admin Panel**: ASP.NET Core MVC + Razor Pages
- **Client Portal**: Angular 21
- **Styling**: Bootstrap 5

### DevOps
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit, Moq, InMemory Database
- **CI/CD**: Automated testing in Docker pipeline

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for Angular client)
- [Docker & Docker Compose](https://www.docker.com/) (for containerized deployment)
- [PostgreSQL 15](https://www.postgresql.org/) (for local development)

## 🚀 Quick Start

### Option 1: Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/Firmeza.git
   cd Firmeza
   ```

2. **Run with Docker Compose**
   ```bash
   docker compose up --build
   ```

3. **Access the applications**
   - API: http://localhost:5277
   - Admin Panel: http://localhost:5000
   - Client Portal: http://localhost:4200

The Docker Compose setup automatically:
- Runs all unit tests before deployment
- Sets up PostgreSQL database
- Initializes the database with seed data
- Starts all services

### Option 2: Local Development

#### 1. Database Setup

```bash
# Create PostgreSQL database
createdb -U postgres FirmezaDB

# Update connection string in appsettings.json files
```

#### 2. Run Migrations

```bash
cd Firmeza.API
dotnet ef database update
```

#### 3. Run the API

```bash
cd Firmeza.API
dotnet run
```

#### 4. Run the Admin Panel

```bash
cd Firmeza.web
dotnet run
```

#### 5. Run the Client Portal

```bash
cd Firmeza.Client
npm install
ng serve
```

## 🧪 Testing

### Run All Tests

```bash
dotnet test Firmeza.Tests/Firmeza.Tests.csproj
```

### Test Coverage

The test suite includes:
- **Unit Tests**: Product commands/queries, Services (Sale, Auth)
- **Integration Tests**: Repository tests with InMemory database
- **DTO Validation Tests**: Client and Product DTOs

Current test count: **24 tests** (all passing)

## 📁 Project Structure

### Firmeza.Domain
Core business entities and repository interfaces. No dependencies on other layers.

**Key Entities:**
- `Product`: Product catalog management
- `Sale`: Sales transactions
- `SaleDetail`: Line items for sales
- `ApplicationUser`: Extended Identity user with custom properties

### Firmeza.Application
Business logic, DTOs, and application services.

**Features:**
- Product CQRS commands and queries
- Sale service for metrics and reporting
- Bulk import service for Excel files
- PDF generation service
- Email service

### Firmeza.Infrastructure
Data access and external service implementations.

**Components:**
- `ApplicationDbContext`: EF Core database context
- Repositories: Product and Sale repositories
- Services: PDF, Email, Bulk Import implementations

### Firmeza.Identity
Authentication and user management.

**Components:**
- `AuthService`: User registration and login
- `JwtTokenService`: JWT token generation
- Identity configuration

### Firmeza.API
RESTful API with JWT authentication.

**Endpoints:**
- `/api/auth`: Authentication (login, register)
- `/api/product`: Product management
- `/api/sales`: Sales operations

### Firmeza.web
Admin panel built with ASP.NET Core MVC.

**Features:**
- Dashboard with sales metrics
- Product management (CRUD, bulk import, export)
- Sales management
- Client management
- User administration

### Firmeza.Client
Angular-based client portal.

**Features:**
- Product catalog browsing
- Shopping cart
- Order history
- User profile management

### Firmeza.Tests
Comprehensive test suite with unit and integration tests.

## 🔐 Default Credentials

After running migrations, the system creates a default admin user:

- **Email**: admin@firmeza.com
- **Password**: Admin123!
- **Role**: Admin

## 🌐 API Documentation

### Authentication

**Register**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "documentId": "123456789",
  "phoneNumber": "1234567890",
  "address": "123 Main St"
}
```

**Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

### Products

**Get All Products**
```http
GET /api/product
Authorization: Bearer {token}
```

**Create Product**
```http
POST /api/product
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product Description",
  "category": "Category",
  "price": 99.99,
  "stock": 100,
  "imageUrl": "https://example.com/image.jpg"
}
```

## 🐳 Docker Configuration

### Services

- **db**: PostgreSQL 15 database
- **tests**: Runs xUnit tests before deployment
- **api**: RESTful API service
- **admin**: Admin panel web application
- **client**: Angular client application

### Environment Variables

Configure in `docker-compose.yml` or `.env` file:

```env
POSTGRES_USER=coder
POSTGRES_PASSWORD=Qwe.123*
POSTGRES_DB=FirmezaDB
JwtSettings__SecretKey=YourSecretKeyHere
```

## 📊 Database Schema

### Main Tables

- **AspNetUsers**: User accounts (Identity)
- **AspNetRoles**: User roles
- **Products**: Product catalog
- **Sales**: Sales transactions
- **SaleDetails**: Sale line items

### Relationships

- `Sale` → `AspNetUsers` (Many-to-One)
- `Sale` → `SaleDetails` (One-to-Many)
- `SaleDetail` → `Product` (Many-to-One)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**David Felipe Vargas Varela**

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Angular team for the powerful frontend framework
- QuestPDF for PDF generation capabilities
- EPPlus for Excel processing
