using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuFlowApi.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MottuFlowApi.Services;
using MottuFlowApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configuração do DbContext
// ----------------------
var environment = builder.Environment.EnvironmentName;
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
var oracleConnectionString = builder.Configuration.GetConnectionString("OracleDb");

if (environment.Equals("Testing", StringComparison.OrdinalIgnoreCase) ||
    AppDomain.CurrentDomain.FriendlyName.Contains("testhost", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine(" Modo de TESTE detectado — usando banco InMemory.");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("MottuFlowTestDb"));
}
else if (useInMemory)
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
        new HeaderApiVersionReader("x-api-version"));
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ----------------------
// Configuração JWT
// ----------------------
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication(options =>
{
    // Define esquemas explícitos (melhor compatibilidade em testes e Swagger)
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Permite teste local e evita falhas em ambiente sem HTTPS
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // evita expiração precoce em testes
    };
});

builder.Services.AddAuthorization();

// ----------------------
// Configuração do Swagger com Versionamento
// ----------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MottuFlow API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de frotas de motocicletas - MottuFlow Sprint 4",
        Contact = new OpenApiContact
        {
            Name = "Equipe MottuFlow",
            Email = "contato@mottuflow.com"
        },
        License = new OpenApiLicense
        {
            Name = "FIAP - Advanced Business Development with .NET"
        }
    });

    // Autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer {seu token JWT}' para autenticar"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Filtros personalizados
    options.DocumentFilter<Documentacao>();
    options.DocumentFilter<OrdenarTagsDocumentFilter>();
    options.EnableAnnotations();
});

// ----------------------
// Health Checks
// ----------------------
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("BancoOracle");

// ----------------------
// Controllers / Endpoints
// ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MottuFlow API v1");
        options.RoutePrefix = "swagger";
    });
}

// Redireciona a raiz para o Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Endpoints de health check
app.MapGet("/api/health/ping", () => Results.Ok(new { status = "API rodando" }));
app.MapHealthChecks("/api/health");

// Controllers
app.MapControllers();

app.Run();

// Necessário para testes de integração com WebApplicationFactory
public partial class Program { }
