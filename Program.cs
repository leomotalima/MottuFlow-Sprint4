using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using Microsoft.OpenApi.Models;

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
builder.Services.AddControllers();

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

    // 🔹 Garante que cada controller seja agrupado por tag
    c.TagActionsBy(api => new[]
    {
        api.GroupName ??
        api.ActionDescriptor.RouteValues["controller"] ??
        "Outros"
    });

    // 🔹 Define a ordem desejada no Swagger
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
        var tag = apiDesc.GroupName ??
                  apiDesc.ActionDescriptor.RouteValues["controller"] ??
                  "Outros";

        var index = Array.IndexOf(ordemDesejada, tag);
        return index == -1 ? int.MaxValue.ToString() : index.ToString("D2");
    });

    c.EnableAnnotations();
});

var app = builder.Build();

// ----------------------
// Middlewares
// ----------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API V1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();


