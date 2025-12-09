using DotNet.Testcontainers.Containers;
using Testcontainers.AutoSetup.Core.Abstractions;

namespace Testcontainers.AutoSetup.EntityFramework;

public class EfSeeder : IDbSeeder
{
    public Task SeedAsync(IContainer container, string connectionString, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
