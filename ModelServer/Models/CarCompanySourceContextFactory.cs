using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ModelServer.Models
{
    public class CarCompanySourceContextFactory : IDesignTimeDbContextFactory<CarCompanySourceContext>
    {
        public CarCompanySourceContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CarCompanySourceContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-2MGQ8LS;Database=CarORM;Trusted_Connection=True;TrustServerCertificate=True;");

            return new CarCompanySourceContext(optionsBuilder.Options);
        }
    }
}
