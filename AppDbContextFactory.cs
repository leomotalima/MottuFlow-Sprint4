using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using System;

namespace MottuFlowApi.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            Env.Load(); // carrega .env

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__OracleConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string não encontrada.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseOracle(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
