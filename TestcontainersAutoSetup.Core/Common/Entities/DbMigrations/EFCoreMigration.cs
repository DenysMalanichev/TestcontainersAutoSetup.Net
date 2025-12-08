using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Migrators.MigrationManagers;

namespace TestcontainersAutoSetup.Core.Common.Entities.DbMigrationTypes;

public class EFCoreMigration<TContext> : DbSetup
    where TContext : DbContext
{
    public EFCoreMigration()
    {
        MigrationType = Enums.MigrationType.EFCore;
    }

    public override Task ExecuteAsync(
        IMigrationRunner migrationRunner, IContainer container, DatabaseConnection connection)
    {
        // TODO add dump logic
        return migrationRunner.ApplyEfCoreMigrationAsync<TContext>(container, connection);
    }
}
