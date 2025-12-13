

using DotNet.Testcontainers.Containers;

namespace Testcontainers.AutoSetup.Core.Abstractions;

public interface IDbRestorer
{
    Task SnapshotAsync(IContainer container, string connectionString, CancellationToken cancellationToken = default);
    Task RestoreAsync(IContainer container, string connectionString, CancellationToken cancellationToken = default);
}