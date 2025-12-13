using DotNet.Testcontainers.Containers;
using Testcontainers.AutoSetup.Core.Abstractions;

namespace Testcontainers.AutoSetup.Core.DbRestoration;

public class MsSqlDbRestorer : DbRestorer
{
    /// <summary>
    /// Sets the default path for restoration snapshot
    /// </summary>
    public MsSqlDbRestorer() : base() { }
    public MsSqlDbRestorer(string restorationStateFilesPath) : base(restorationStateFilesPath) { }

    public override Task RestoreAsync(IContainer container, string connectionStrings, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task SnapshotAsync(IContainer container, string connectionStrings, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
