using DotNet.Testcontainers.Containers;

namespace Testcontainers.AutoSetup.Core.Abstractions;

public abstract class DbRestorer : IDbRestorer
{
    protected readonly string _restorationStateFilesPath;

    public DbRestorer(string restorationStateFilesPath = "./Restoration")
    {
        // TODO validate if a path is real
        if(string.IsNullOrEmpty(restorationStateFilesPath))
        {
            throw new ArgumentNullException(nameof(restorationStateFilesPath));   
        }
        _restorationStateFilesPath = restorationStateFilesPath;
    }

    public abstract Task RestoreAsync(IContainer container, string connectionStrings, CancellationToken cancellationToken = default);

    public abstract Task SnapshotAsync(IContainer container, string connectionStrings, CancellationToken cancellationToken = default);
}