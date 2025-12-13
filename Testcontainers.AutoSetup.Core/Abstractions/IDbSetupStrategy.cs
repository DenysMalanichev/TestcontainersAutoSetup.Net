
using DotNet.Testcontainers.Containers;

namespace Testcontainers.AutoSetup.Core.Abstractions;

public interface IDbSetupStrategy
{
    Task InitializeGlobalAsync(
        IContainer container, 
        string containerConnectionString,
        CancellationToken cancellationToken = default);
}
