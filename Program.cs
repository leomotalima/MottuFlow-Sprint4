using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuFlowApi.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configuração do DbContext
// ----------------------
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
var oracleConnectionString = builder.Configuration.GetConnectionString("OracleDb");

if (useInMemory)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("MottuFlowDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(oracleConnectionString));
}

// ----------------------
// Versionamento de API
// ----------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version")
    );
});

// ----------------------
// Configuração do Swagger
// ----------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MottuFlow API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de frotas de motocicletas - MottuFlow Sprint 4"
    });

    // Aplica filtro para ordenar tags
    c.DocumentFilter<OrdenarTagsDocumentFilter>();
});

// ----------------------
// Health Checks
// ----------------------
builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API v1");
        c.RoutePrefix = string.Empty; // Swagger abre direto na raiz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoint simples para Health Check
app.MapGet("/api/health/ping", () =>
{
    return Results.Ok(new { status = "API rodando 🚀" });
});

app.MapControllers();
app.MapHealthChecks("/api/health");

app.Run();
