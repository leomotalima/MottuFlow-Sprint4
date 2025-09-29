using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

public class OrdenarTagsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Ordem desejada das tags
        string[] ordemTags = { "Funcionario", "Patio", "Moto", "Camera", "Arucotag", "Localidade", "Registro_status" };

        // Reordena as tags
        swaggerDoc.Tags = swaggerDoc.Tags?
            .OrderBy(t =>
            {
                int index = Array.IndexOf(ordemTags, t.Name);
                return index >= 0 ? index : int.MaxValue;
            })
            .ToList();

        // Reordena os paths/endpoints dentro das tags
        var orderedPaths = new OpenApiPaths();
        foreach (var tag in ordemTags)
        {
            foreach (var path in swaggerDoc.Paths
                        .Where(p => p.Value.Operations.Any(o => o.Value.Tags.Any(tg => tg.Name == tag))))
            {
                if (!orderedPaths.ContainsKey(path.Key))
                    orderedPaths.Add(path.Key, path.Value);
            }
        }

        // Adiciona quaisquer paths restantes que não estão na lista
        foreach (var path in swaggerDoc.Paths)
        {
            if (!orderedPaths.ContainsKey(path.Key))
                orderedPaths.Add(path.Key, path.Value);
        }

        swaggerDoc.Paths = orderedPaths;
    }
}
