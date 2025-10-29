using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            // Define ambiente de teste
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove o DbContext do Oracle
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Remove também o AppDbContext se estiver registrado
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));
                
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Adiciona o banco InMemory para testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });

            // Configura após todos os serviços
            builder.ConfigureServices(services =>
            {
                // Garante que o banco seja populado após a criação
                var sp = services.BuildServiceProvider();
                
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Recria o banco
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Popula dados de teste
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