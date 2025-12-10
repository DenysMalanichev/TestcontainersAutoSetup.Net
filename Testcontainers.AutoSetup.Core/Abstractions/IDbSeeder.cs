using DotNet.Testcontainers.Containers;

namespace Testcontainers.AutoSetup.Core.Abstractions;

public interface IDbSeeder
{
    Task SeedAsync(IContainer container, string connectionString, CancellationToken cancellationToken);
}