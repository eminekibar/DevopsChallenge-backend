var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Swagger sadece dev'de kalsın
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// ✅ health endpoint (probe'lar buna bakacak)
app.MapHealthChecks("/healthz");

app.MapControllers();
app.Run();

