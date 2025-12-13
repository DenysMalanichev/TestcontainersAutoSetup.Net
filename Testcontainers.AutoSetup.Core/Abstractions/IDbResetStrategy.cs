using DotNet.Testcontainers.Containers;

namespace Testcontainers.AutoSetup.Core.Abstractions;

public interface IDbResetStrategy
{
    Task ResetAsync(
        IContainer container,
        string containerConnectionString,
        CancellationToken cancellationToken = default);
}
