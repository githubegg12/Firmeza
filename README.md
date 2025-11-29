# Firmeza Project

This project is a comprehensive sales and inventory management system built with ASP.NET Core.

## Environment Variables

To run this project successfully, you need to configure the following environment variables. You can set these in your `appsettings.json` files or as environment variables in your deployment environment (e.g., Docker, Azure, AWS).

### Database Configuration

Used by both API and Web projects to connect to the PostgreSQL database.

| Variable | Description | Example Value |
| :--- | :--- | :--- |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string. | `Host=localhost;Port=5432;Database=FirmezaDB;Username=coder;Password=Qwe.123*` |

### JWT Configuration (API Only)

Required for generating and validating JSON Web Tokens for authentication.

| Variable | Description | Example Value |
| :--- | :--- | :--- |
| `JwtSettings__SecretKey` | Secret key for signing tokens. Must be at least 32 characters. | `YourSuperSecretKey1234567890!@#$%` |
| `JwtSettings__Issuer` | The issuer of the token. | `FirmezaAPI` |
| `JwtSettings__Audience` | The intended audience for the token. | `FirmezaClients` |
| `JwtSettings__ExpirationMinutes` | Token expiration time in minutes. | `60` |

### Email Configuration

Used for sending welcome emails, purchase confirmations, etc.

| Variable | Description | Example Value |
| :--- | :--- | :--- |
| `EmailSettings__SmtpHost` | SMTP server host address. | `smtp.gmail.com` |
| `EmailSettings__SmtpPort` | SMTP server port. | `587` |
| `EmailSettings__EnableSsl` | Whether to use SSL/TLS. | `true` |
| `EmailSettings__SenderEmail` | Email address sending the emails. | `your-email@gmail.com` |
| `EmailSettings__SenderName` | Display name for the sender. | `Firmeza System` |
| `EmailSettings__Username` | SMTP username (usually same as sender email). | `your-email@gmail.com` |
| `EmailSettings__Password` | SMTP password or App Password. | `your-app-password` |

## Docker Configuration

If running with Docker Compose, the `docker-compose.yml` file already sets up default values for development.

- **PostgreSQL Database**:
  - `POSTGRES_USER`: `coder`
  - `POSTGRES_PASSWORD`: `Qwe.123*`
  - `POSTGRES_DB`: `FirmezaDB`

- **API Service**:
  - Overrides `ConnectionStrings__DefaultConnection` to point to the `db` container.
  - Sets default `JwtSettings`.

- **Admin Web Service**:
  - Overrides `ConnectionStrings__DefaultConnection` to point to the `db` container.

## Running the Application

1.  **Prerequisites**: Ensure you have .NET 8 SDK and Docker installed.
2.  **Configuration**: Update `appsettings.json` in `Firmeza.API` and `Firmeza.web` with your specific settings (especially Email credentials).
3.  **Run with Docker**:
    ```bash
    docker compose up --build
    ```
4.  **Access**:
    - **API**: http://localhost:5277/swagger
    - **Admin Panel**: http://localhost:5000
    - **Client App**: http://localhost:4200
