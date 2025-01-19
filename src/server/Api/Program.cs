using Microsoft.EntityFrameworkCore;
using Database;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Resolve the database path
var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
var dbPath = Path.Combine(solutionRoot, "src", "database", "database.db");
Console.WriteLine($"Resolved Database Path: {dbPath}");

// Configure the database context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
           .LogTo(Console.WriteLine, LogLevel.Information)); // Enable EF Core logs

builder.Services.AddScoped<GameService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:5071") // URL of Blazor client
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    try
    {
        Console.WriteLine("Applying migrations...");
        dbContext.Database.Migrate(); // Ensure database is created and up-to-date
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during migration: {ex.Message}");
        throw;
    }
}

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
