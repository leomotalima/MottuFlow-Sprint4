using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc; // necessário para [Tags]

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configurações do DbContext
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
// Add services to the container
// ----------------------
builder.Services.AddControllers();

// ----------------------
// Swagger/OpenAPI
// ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MottuFlow API",
        Version = "v1",
        Description = "API para gerenciamento do fluxo de motos e registros de status",
    });

    c.EnableAnnotations();
    c.CustomSchemaIds(type => type.FullName);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header (exemplo)"
    });

    // 🔽 Ordem personalizada das seções do Swagger
    var ordem = new List<string>
    {
        "Funcionario",
        "Patio",
        "Moto",
        "Camera",
        "ArucoTag",
        "Localidade",
        "RegistroStatus"
    };

    // Garante que as tags sejam atribuídas corretamente
    c.TagActionsBy(api =>
    {
        // Se tiver [Tags("Nome")] no controller ou método, usa isso
        var tag = api.ActionDescriptor.EndpointMetadata
            .OfType<TagsAttribute>()
            .FirstOrDefault()?.Tags?.FirstOrDefault();

        // Se não, tenta pegar do GroupName
        if (string.IsNullOrEmpty(tag))
            tag = api.GroupName;

        // Se ainda não tiver, usa a primeira parte da rota (ex: "funcionarios")
        if (string.IsNullOrEmpty(tag) && api.RelativePath != null)
            tag = api.RelativePath.Split('/')[1];

        return new[] { tag ?? "Outros" };
    });

    // Ordena os grupos de acordo com a lista "ordem"
    c.OrderActionsBy(apiDesc =>
    {
        var tag = apiDesc.GroupName ??
                  apiDesc.RelativePath?.Split('/')[1] ??
                  "Outros";

        var index = ordem.IndexOf(tag);
        return index >= 0 ? index.ToString("D2") : tag;
    });
});

// ----------------------
// Registrar Services e Repositories automaticamente
// ----------------------
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes();

// Services
foreach (var type in types)
{
    if (type.IsInterface && type.Name.StartsWith("I") && type.Namespace == "MottuFlowApi.Services")
    {
        var implementation = types.FirstOrDefault(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t));
        if (implementation != null)
            builder.Services.AddScoped(type, implementation);
    }
}

// Repositories
foreach (var type in types)
{
    if (type.IsInterface && type.Name.StartsWith("I") && type.Namespace == "MottuFlowApi.Repositories")
    {
        var implementation = types.FirstOrDefault(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t));
        if (implementation != null)
            builder.Services.AddScoped(type, implementation);
    }
}

// ----------------------
// Build app
// ----------------------
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API V1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();
