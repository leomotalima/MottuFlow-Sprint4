using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MottuFlowApi.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Connection string hardcoded para design-time
            optionsBuilder.UseOracle("User Id=rm557851;Password=020382;Data Source=localhost:1521/ORCL;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
