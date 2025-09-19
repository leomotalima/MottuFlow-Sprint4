using Microsoft.EntityFrameworkCore;
using MottuFlowApi.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Add services to the container
// ----------------------
builder.Services.AddControllers();

// DbContext - InMemory para testes locais
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("MottuFlowDb"));

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

    // Habilita o uso das anotações [SwaggerOperation]
    c.EnableAnnotations();

    // Garante que nomes duplicados não quebrem o Swagger
    c.CustomSchemaIds(type => type.FullName);

    // Adiciona definição de segurança mínima para .NET 10 preview
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header (exemplo)"
    });
});

// ----------------------
// Registrar automaticamente Services e Repositories
// ----------------------
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes();

// Registrar Services
foreach (var type in types)
{
    if (type.IsInterface && type.Name.StartsWith("I") && type.Namespace == "MottuFlowApi.Services")
    {
        var implementation = types.FirstOrDefault(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t));
        if (implementation != null)
            builder.Services.AddScoped(type, implementation);
    }
}

// Registrar Repositories
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

// Enable Swagger UI sempre (não apenas em Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API V1");
});

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
