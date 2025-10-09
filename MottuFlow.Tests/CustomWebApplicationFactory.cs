using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MottuFlowApi;
using MottuFlowApi.Data;

namespace MottuFlow.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o contexto de banco real (Oracle)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona o banco InMemory apenas para os testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Cria e inicializa o banco com dados básicos
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    if (!db.Funcionarios.Any())
                    {
                        db.Funcionarios.Add(new MottuFlowApi.Models.Funcionario
                        {
                            Nome = "Léo Mota Lima",
                            CPF = "12345678900",
                            Cargo = "Desenvolvedor",
                            Telefone = "11999999999",
                            Email = "leo@mottuflow.com",
                            Senha = "123456"
                        });
                        db.SaveChanges();
                    }
                }
            });
        }
    }
}
