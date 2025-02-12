# .net-api-example-with-jwt

# Todo API with JWT Authentication

A .NET Core Web API project implementing a secure Todo application with JWT authentication and refresh tokens.

## Features

- User authentication with JWT tokens
- Refresh token mechanism
- Password hashing using BCrypt
- User-specific todos
- RESTful API endpoints
- MySQL database integration
- Swagger UI for API documentation

## Prerequisites

- .NET 8.0 SDK
- MySQL Server
- IDE (Recommended: JetBrains Rider or Visual Studio Code)

## Getting Started

1. Clone the repository
2. Install dependencies:
```bash
dotnet restore
```

3. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=todoapp;user=root;password=your_password"
  }
}
```

4. Run database migrations:
```bash
dotnet ef database update
```

5. Run the application:
```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or similar port shown in the console)

## API Endpoints

### Authentication

- POST `/auth/register` - Register new user
- POST `/auth/login` - Login user
- POST `/auth/refresh-token` - Refresh access token
- POST `/auth/logout` - Logout user

### Todos

- GET `/todo` - Get all todos for authenticated user
- GET `/todo/{id}` - Get specific todo
- POST `/todo` - Create new todo
- PUT `/todo/{id}` - Update todo
- DELETE `/todo/{id}` - Delete todo

## Security Features

- Secure password hashing with BCrypt
- JWT token authentication
- Refresh token mechanism
- User-specific data access
- Password validation requirements:
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one number
  - At least one special character

## Database Schema

### Users Table
- Id (int, primary key)
- Username (string)
- Password (string, hashed)

### RefreshTokens Table
- Id (int, primary key)
- Token (string)
- ExpiryDate (DateTime)
- IsRevoked (bool)
- UserId (int, foreign key)

### Todos Table
- Id (int, primary key)
- Title (string)
- IsComplete (bool)
- UserId (int, foreign key)

## Example Usage

### Register User
```bash
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"SecurePass123!"}'
```

### Login
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"SecurePass123!"}'
```

### Create Todo
```bash
curl -X POST http://localhost:5000/todo \
  -H "Authorization: Bearer your_token_here" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Todo","isComplete":false}'
```

## Development

### Add New Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Reset Database
```bash
dotnet ef database drop -f
dotnet ef database update
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details
