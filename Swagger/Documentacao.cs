using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace MottuFlow.Swagger
{
    public class Documentacao : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Funcionários", Description = "Gerencia os dados dos funcionários, incluindo autenticação e registros de status" },
                new OpenApiTag { Name = "Pátios", Description = "Gerencia os pátios, suas capacidades e câmeras associadas" },
                new OpenApiTag { Name = "Motos", Description = "Gerencia as motos cadastradas no sistema, incluindo modelo, placa e localização" },
                new OpenApiTag { Name = "Câmeras", Description = "Gerencia as câmeras de monitoramento dentro dos pátios" },
                new OpenApiTag { Name = "ArucoTags", Description = "Controle de ArUco Tags associadas às motos para rastreamento" },
                new OpenApiTag { Name = "Localidades", Description = "Registra a posição das motos capturada pelas câmeras nos pátios" },
                new OpenApiTag { Name = "Status", Description = "Histórico de status das motos, registrado por funcionários" }
            };
        }
    }
}
