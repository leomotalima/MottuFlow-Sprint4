using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MottuFlowApi;
using MottuFlowApi.Data;
using MottuFlowApi.Models;
using System.Linq;

namespace MottuFlow.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o contexto de banco real (ex: Oracle)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona o banco InMemory para os testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Cria um escopo temporário para configurar o banco
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Limpa e recria o banco antes de popular
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Popula dados iniciais se o banco estiver vazio
                if (!db.Funcionarios.Any())
                {
                    db.Funcionarios.Add(new Funcionario
                    {
                        Nome = "Léo Mota Lima",
                        CPF = "12345678900",
                        Cargo = "Desenvolvedor",
                        Telefone = "11999999999",
                        Email = "leo@mottuflow.com",
                        Senha = BCrypt.Net.BCrypt.HashPassword("123456")
                    });

                    db.SaveChanges();
                }
            });
        }
    }
}
