using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

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
// Controllers
// ----------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Evita problemas de referência circular no Swagger
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ----------------------
// Swagger
// ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MottuFlow API",
        Version = "v1",
        Description = "API para gerenciamento do fluxo de motos e registros de status"
    });

    // Agrupamento por controller
    c.TagActionsBy(api => new[]
    {
        api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Outros"
    });

    // Ordem customizada
    var ordemDesejada = new[]
    {
        "Funcionario",
        "Patio",
        "Moto",
        "Camera",
        "ArucoTag",
        "Localidade",
        "RegistroStatus"
    };

    c.OrderActionsBy(apiDesc =>
    {
        var tag = apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Outros";
        var index = Array.IndexOf(ordemDesejada, tag);
        return index == -1 ? int.MaxValue.ToString() : index.ToString("D2");
    });

    // Ativa anotações [SwaggerOperation]
    c.EnableAnnotations();

    
});


// ----------------------
// Build da aplicação
// ----------------------
var app = builder.Build();

// ----------------------
// Porta fixa
// ----------------------
app.Urls.Clear();
app.Urls.Add("http://localhost:5224");

// ----------------------
// Middleware de exceção detalhada (somente DEV)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// ----------------------
// Middlewares Swagger
// ----------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API V1");
});

// ----------------------
// Middlewares adicionais
// ----------------------
app.UseAuthorization();
app.MapControllers();

// ----------------------
// Executa aplicação
// ----------------------
app.Run();
