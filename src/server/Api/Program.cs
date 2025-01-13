using Microsoft.EntityFrameworkCore;
using Database; 
using Api.Services; 

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName; // Aller à la racine du projet
var dbPath = Path.Combine(solutionRoot, "src", "database", "database.db");
Console.WriteLine($"Resolved Database Path: {dbPath}");

if (!File.Exists(dbPath))
{
    Console.WriteLine($"Error: Database file not found at {dbPath}. Ensure the database exists or run migrations.");
    Environment.Exit(1); 
}

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));


builder.Services.AddScoped<GameService>(); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:5071") // URL du client Blazor
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

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
