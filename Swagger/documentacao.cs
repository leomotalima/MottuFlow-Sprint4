using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MottuFlowApi.Swagger
{
    /// <summary>
    /// Define as informações gerais e descrições de cada tag do Swagger.
    /// </summary>
    public class Documentacao : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // 🧩 Informações principais da API
            swaggerDoc.Info = new OpenApiInfo
            {
                Title = "MottuFlow API",
                Version = "v1",
                Description = "API RESTful desenvolvida para gerenciamento de frotas, pátios, funcionários e monitoramento com câmeras, seguindo as boas práticas HTTP, REST e arquitetura em camadas.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe MottuFlow",
                    Email = "contato@mottuflow.com"
                },
                License = new OpenApiLicense
                {
                    Name = "FIAP - Advanced Business Development with .NET",
                    Url = new Uri("https://www.fiap.com.br")
                }
            };

            // 🗂️ Descrições das tags exibidas no Swagger
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = "Funcionários", Description = "Gerencia os dados dos funcionários (CRUD completo com HATEOAS e autenticação)." },
                new OpenApiTag { Name = "Pátios", Description = "Gerencia os pátios e suas capacidades de armazenamento de motos." },
                new OpenApiTag { Name = "Motos", Description = "Gerencia as motos cadastradas, incluindo modelo, placa e status operacional." },
                new OpenApiTag { Name = "Câmeras", Description = "Gerencia as câmeras de monitoramento instaladas nos pátios." },
                new OpenApiTag { Name = "ArucoTags", Description = "Gerencia as ArUco Tags associadas às motos para identificação visual." },
                new OpenApiTag { Name = "Localidades", Description = "Gerencia os registros de localização e deslocamento das motos." },
                new OpenApiTag { Name = "Registros de Status", Description = "Gerencia registros automáticos de status capturados pelas câmeras." },

            };
        }
    }
}
