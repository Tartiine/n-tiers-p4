using Microsoft.EntityFrameworkCore;
using Api.Data;

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName; // Go back to the root
var dbPath = Path.Combine(solutionRoot, "src", "database", "database.db");
Console.WriteLine($"Resolved Database Path: {dbPath}");

// Ensure the directory and file exist (for debugging)
if (!File.Exists(dbPath))
{
    Console.WriteLine($"Error: Database file not found at {dbPath}");
}

// Configure database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", builder =>
    {
        builder.WithOrigins("http://localhost:5071") 
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure middleware
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
