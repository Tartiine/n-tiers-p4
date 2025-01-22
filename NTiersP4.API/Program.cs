using NTiersP4.Domain;
using NTiersP4.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
var dbPath = Path.Combine(solutionRoot, "NTiersP4.Infrastructure", "database.db");

// For Entity Framework and DI
builder.Services
    .AddInfrastructure(dbPath)
    .AddDomain();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
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