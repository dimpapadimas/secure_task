using Microsoft.EntityFrameworkCore;
using Serilog;
using SecureTaskApi.Data;
using SecureTaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

Console.WriteLine("🚀 Starting Secure Task API...");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Secure Task API", 
        Version = "v1",
        Description = "A sample task management API for DevSecOps pipeline testing"
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tasks.db";
Console.WriteLine($"📊 Using database: {connectionString}");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(connectionString));

// Services
builder.Services.AddScoped<ITaskService, TaskService>();

// Memory Cache
builder.Services.AddMemoryCache();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlite(connectionString);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

Console.WriteLine("🔧 Initializing database...");

// Initialize database
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        db.Database.EnsureCreated();
        Console.WriteLine("✅ Database initialized successfully");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("📚 Swagger UI enabled");
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Redirect("/swagger"));

var urls = app.Urls;
Console.WriteLine("🌐 Application URLs:");
foreach (var url in urls)
{
    Console.WriteLine($"   {url}");
}

Console.WriteLine("✨ Application started successfully!");
Console.WriteLine("📖 Visit http://localhost:5000/swagger for API documentation");
Console.WriteLine("💚 Visit http://localhost:5000/health for health check");
Console.WriteLine();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
