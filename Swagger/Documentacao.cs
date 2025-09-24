using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class Documentacao : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "Funcionários", Description = "Gerencia os dados dos funcionários" },
            new OpenApiTag { Name = "Pátios", Description = "Gerencia os pátios e suas capacidades" },
            new OpenApiTag { Name = "Motos", Description = "Gerencia as motos cadastradas no sistema" },
            new OpenApiTag { Name = "Câmeras", Description = "Gerencia as câmeras de monitoramento" },
            new OpenApiTag { Name = "ArucoTags", Description = "Controle de ArUco Tags ligadas às motos" },
            new OpenApiTag { Name = "Status", Description = "Histórico de status das motos" },
            new OpenApiTag { Name = "Localidades", Description = "Registros de localização das motos" }
        };
    }
}
