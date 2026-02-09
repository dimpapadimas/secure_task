# Quick Start Guide - Secure Task API

## 🚀 First Steps

### 1. Restore & Build
```bash
cd /Users/tyler/Developer/eu_council/playground/SecureTaskApi
dotnet restore
dotnet build
```

### 2. Run the Application
```bash
dotnet run
```
Visit: https://localhost:5001/swagger

### 3. Run Tests
```bash
cd ../SecureTaskApi.Tests
dotnet test --logger "console;verbosity=detailed"
```

### 4. Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

## 🔍 Security Scanning Commands

### Check for Vulnerable Packages
```bash
dotnet list package --vulnerable
dotnet list package --vulnerable --include-transitive
```

### Generate SBOM (CycloneDX)
```bash
# Install the tool first
dotnet tool install --global CycloneDX

# Generate SBOM
dotnet CycloneDX SecureTaskApi/SecureTaskApi.csproj -o ./sbom -f json
```

### OWASP Dependency-Check (if installed)
```bash
dependency-check --project "SecureTaskApi" \
  --scan . \
  --format HTML \
  --out ./dependency-check-report
```

## 🐳 Docker Commands

### Build Image
```bash
cd SecureTaskApi
docker build -t securetaskapi:latest .
```

### Run Container
```bash
docker run -d -p 8080:8080 --name taskapi securetaskapi:latest
```

### Scan with Trivy
```bash
trivy image securetaskapi:latest
```

### Scan with Grype
```bash
grype securetaskapi:latest
```

## 📊 Code Quality

### Code Coverage Report (ReportGenerator)
```bash
# Install tool
dotnet tool install --global dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html
```

## 🎯 Testing the API

### Create a Task
```bash
curl -X POST https://localhost:5001/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test GitHub Actions Pipeline",
    "description": "Create and test CI/CD workflow",
    "priority": 2
  }'
```

### Get All Tasks
```bash
curl https://localhost:5001/api/tasks
```

### Get Statistics
```bash
curl https://localhost:5001/api/tasks/statistics
```

## 📝 Project Stats

- **Language**: C# / .NET 8
- **Dependencies**: 13 NuGet packages
- **Test Coverage Target**: >80%
- **Lines of Code**: ~500+ (good for scanning demos)

## 🎓 Next Steps for Pipeline Learning

1. ✅ **Week 1 - Challenge 2**: Basic .NET pipeline (already created!)
2. ⬜ **Week 1 - Challenge 3**: Add matrix builds (test on .NET 6, 7, 8)
3. ⬜ **Week 2 - Challenge 5**: Integrate SonarCloud
4. ⬜ **Week 2 - Challenge 6**: Add dependency scanning
5. ⬜ **Week 2 - Challenge 8**: Container scanning with Trivy

## 🔧 Useful .NET Commands

```bash
# Clean build artifacts
dotnet clean

# Watch mode (auto-rebuild on changes)
dotnet watch run

# List all packages
dotnet list package

# Update packages
dotnet add package <PackageName>

# Remove package
dotnet remove package <PackageName>
```

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Change port in Properties/launchSettings.json
# or use:
dotnet run --urls "https://localhost:7001"
```

### Database Locked
```bash
# Delete the SQLite database
rm tasks.db tasks.db-shm tasks.db-wal
```

### Restore Issues
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
dotnet restore --force
```

---

**Happy Pipeline Building! 🚀**
