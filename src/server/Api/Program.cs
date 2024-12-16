using Microsoft.EntityFrameworkCore;
using Database; // Pour accéder à DatabaseContext
using Api.Services; // Pour le service GameService

var builder = WebApplication.CreateBuilder(args);

// Résoudre le chemin de la base de données
var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName; // Aller à la racine du projet
var dbPath = Path.Combine(solutionRoot, "src", "database", "database.db");
Console.WriteLine($"Resolved Database Path: {dbPath}");

// Vérifier l'existence de la base de données
if (!File.Exists(dbPath))
{
    Console.WriteLine($"Error: Database file not found at {dbPath}. Ensure the database exists or run migrations.");
    Environment.Exit(1); // Arrêter l'application si la base de données est absente
}

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Ajouter les services applicatifs
builder.Services.AddScoped<GameService>(); // Service pour les jeux

// Configurer les services pour l'API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurer les CORS pour autoriser le client Blazor
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

// Appliquer les migrations (si nécessaire)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

// Configurer le middleware
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
