
using DotNet.Testcontainers.Containers;
using Testcontainers.AutoSetup.Core.Abstractions;
using Testcontainers.AutoSetup.Core.Common.Entities;

namespace Testcontainers.AutoSetup.Core.Common;

public class DbSetupStrategy : IDbStrategy
{
    private readonly IDbSeeder _seeder;
    private readonly IDbRestorer _restorer;
    private readonly IList<DbSetup> _dbSetups;

    public DbSetupStrategy(IDbSeeder seeder, IDbRestorer restorer, params DbSetup[] dbSetups)
    {
        if(dbSetups is null || dbSetups.Length == 0)
        {
            throw new ArgumentException("DB setups cannot be empty", nameof(dbSetups));
        }
        _seeder = seeder ?? throw new ArgumentNullException(nameof(seeder));
        _restorer = restorer ?? throw new ArgumentNullException(nameof(restorer));
        _dbSetups = dbSetups;
    }

    public async Task InitializeGlobalAsync(
        IContainer container,
        string containerConnectionString,
        CancellationToken cancellationToken = default)
    {
        foreach(var dbSetup in _dbSetups)
        {
            await _seeder.SeedAsync(
                dbSetup,
                container,
                dbSetup.BuildConnectionString(containerConnectionString),
                cancellationToken);
        }
    }

    public async Task ResetAsync(
        IContainer container,
        string containerConnectionString,
        CancellationToken cancellationToken = default)
    {
        foreach(var dbSetup in _dbSetups)
        {
            await _restorer.RestoreAsync(
                container,
                dbSetup.BuildConnectionString(containerConnectionString),
                cancellationToken);
        }
    }
}