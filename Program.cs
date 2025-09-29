using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuFlowApi.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

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
// Configuração do Swagger
// ----------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MottuFlow API", Version = "v1" });

    // Aplica filtro para ordenar tags
    c.DocumentFilter<OrdenarTagsDocumentFilter>();
});

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
