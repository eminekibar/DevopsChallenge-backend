using Microsoft.EntityFrameworkCore;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL bağlantı bilgilerini environment variable'lardan al
var postgresHost = "postgres";
var postgresPort = 5432;
var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "mydb";
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";

var connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDb};Username={postgresUser};Password={postgresPassword}";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// PostgreSQL bağlantısını ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// health endpoint (probe'lar buna bakacak)
app.MapHealthChecks("/healthz");

app.MapControllers();
app.Run();

