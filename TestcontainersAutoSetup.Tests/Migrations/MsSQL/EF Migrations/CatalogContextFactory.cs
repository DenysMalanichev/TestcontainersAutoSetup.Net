using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class CatalogContextFactory : IDesignTimeDbContextFactory<CatalogContext>
{
    public CatalogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>();

        // 1. Use a dummy connection string. (The actual one is used at runtime)
        // 2. IMPORTANT: We must tell EF that the migrations belong to the "Tests" assembly, 
        //    otherwise it will try to put them in the "Infrastructure" assembly where the Context is defined.
        optionsBuilder.UseSqlServer(
            "Data Source=Dummy;Initial Catalog=DummyDb;Integrated Security=True;TrustServerCertificate=True",
            b => b.MigrationsAssembly("TestcontainersAutoSetup.Tests") 
        );

        return new CatalogContext(optionsBuilder.Options);
    }
}