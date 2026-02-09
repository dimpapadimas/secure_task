# Secure Task API

A .NET 8 Web API project for learning DevSecOps pipelines and security scanning tools.

## 🎯 Purpose

This project is designed specifically for testing and learning:
- GitHub Actions CI/CD pipelines
- Dependency scanning (OWASP Dependency-Check, dotnet list package --vulnerable)
- SAST tools (SonarCloud, Security Code Scan)
- Container scanning (Trivy, Grype)
- SBOM generation (CycloneDX, SPDX)
- Code coverage reporting

## 🏗️ Architecture

- **ASP.NET Core 8.0** Web API
- **Entity Framework Core** with SQLite
- **Serilog** for structured logging
- **Swagger/OpenAPI** for API documentation
- **xUnit** for unit testing with Moq and FluentAssertions

## 📦 Key Dependencies (for scanning practice)

The project intentionally includes several popular packages:

- **Web**: Swashbuckle.AspNetCore
- **Database**: Microsoft.EntityFrameworkCore, SQLite
- **Logging**: Serilog.AspNetCore
- **JSON**: Newtonsoft.Json
- **Validation**: FluentValidation
- **Resilience**: Polly
- **Health Checks**: AspNetCore.HealthChecks
- **Testing**: xUnit, Moq, FluentAssertions

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Your favorite IDE (VS Code, Visual Studio, Rider)

### Run Locally

```bash
# Restore dependencies
dotnet restore

# Run the API
cd SecureTaskApi
dotnet run

# API will be available at: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Run Tests

```bash
cd SecureTaskApi.Tests
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## 🔍 API Endpoints

- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `GET /api/tasks/statistics` - Get task statistics
- `GET /health` - Health check endpoint

## 📊 Sample Request

```bash
# Create a new task
curl -X POST http://localhost:5000/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn GitHub Actions",
    "description": "Build a complete DevSecOps pipeline",
    "priority": 2
  }'
```

## 🔐 DevSecOps Pipeline Ideas

Use this project to practice:

1. **SCA (Software Composition Analysis)**
   - `dotnet list package --vulnerable`
   - OWASP Dependency-Check
   - Snyk, WhiteSource, etc.

2. **SAST (Static Application Security Testing)**
   - SonarCloud/SonarQube
   - Security Code Scan
   - Roslyn Analyzers

3. **Container Security**
   - Build Docker image
   - Scan with Trivy
   - Scan with Grype/Syft

4. **SBOM Generation**
   - CycloneDX for .NET
   - Generate SPDX format

5. **Code Quality**
   - Code coverage thresholds
   - Mutation testing with Stryker.NET

## 📁 Project Structure

```
SecureTaskApi/
├── Controllers/       # API Controllers
├── Models/           # Data models and DTOs
├── Services/         # Business logic
├── Data/             # EF Core DbContext
├── Program.cs        # Application entry point
└── appsettings.json  # Configuration

SecureTaskApi.Tests/
└── TaskServiceTests.cs  # Unit tests
```

## 🧪 Testing

The project includes comprehensive unit tests covering:
- CRUD operations
- Business logic validation
- Edge cases
- Error handling

Test coverage target: >80%

## 📝 License

This is a learning project - use it however you like!

## 🎓 Learning Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [OWASP Dependency-Check](https://owasp.org/www-project-dependency-check/)
- [SonarCloud for .NET](https://sonarcloud.io/)
- [Trivy Container Scanner](https://trivy.dev/)
