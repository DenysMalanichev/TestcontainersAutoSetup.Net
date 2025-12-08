using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Testcontainers.Core.Abstractions;
using Testcontainers.MsSql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common;
using TestcontainersAutoSetup.Core.Implementation;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public class SqlServerContainerBuilder : DbContainer, IMigrationRunner
{
    private readonly MsSqlBuilder _msSqlBuilder = new MsSqlBuilder();
    private readonly IServiceProvider _serviceProvider;

    private string? _migrationsPath;
    private string? _snapshotDirectory;

    public SqlServerContainerBuilder(AutoSetupContainerBuilder mainBuilder, IServiceProvider serviceProvider)
        : base(mainBuilder)
    {
        _serviceProvider = serviceProvider;
        if(mainBuilder.DockerEndpoint != null)
            _msSqlBuilder = _msSqlBuilder.WithDockerEndpoint(mainBuilder.DockerEndpoint);
    }

    public override async Task<IContainer> BuildAndInitializeAsync()
    {
        var container = _msSqlBuilder.Build();
        await container.StartAsync();

        foreach (var dbSetup in DbSetups)
        {
            var connection = new DatabaseConnection
            {
                ConnectionString = dbSetup.BuildConnectionString(container.GetConnectionString()),
            };

            await dbSetup.ExecuteAsync(this, container, connection);           
        }

        return container;
    }

    public override DbContainer UseDatabaseName(string dbName)
    {
        if(dbName.IsNullOrEmpty())
        {
            throw new InvalidParameterException("DB name cannot be null or empty.");
        }

        var dbSetup = GetConfiguringDbSetup();
        dbSetup.DbName = dbName;
        return this;
    }

    public async Task ApplyEfCoreMigrationAsync<TContext>(IContainer container, DatabaseConnection connection)
        where TContext : DbContext
    {
        // 1. Create the Options with the NEW Container Connection String
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseSqlServer(connection.ConnectionString);

        // 2. Create a Scope (Best practice for resolving scoped services in EF)
        using var scope = _serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        // 3. MAGIC: ActivatorUtilities uses the Provider to find dependencies (ITenantService),
        // but accepts 'optionsBuilder.Options' as a manual override argument.
        var context = ActivatorUtilities.CreateInstance<TContext>(
            scopedProvider, 
            optionsBuilder.Options
        );

        await context.Database.MigrateAsync();
    }
}