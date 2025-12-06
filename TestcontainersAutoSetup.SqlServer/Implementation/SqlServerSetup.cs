using System.Text;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Testcontainers.Core.Abstractions;
using Testcontainers.MsSql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common;
using TestcontainersAutoSetup.Core.Enums;
using TestcontainersAutoSetup.Core.Implementation;
using IMigrator = TestcontainersAutoSetup.Core.Abstractions.IMigrator;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public class SqlServerSetup : IContainerSetup, IMigrator, IEFCoreMigrator, IEFCoreBuilder
{
    private readonly AutoSetupContainerBuilder _mainBuilder;
    private readonly MsSqlBuilder _msSqlBuilder = new MsSqlBuilder();

    private string? _migrationsPath;
    private string? _dbName;
    private string? _snapshotDirectory;
    private MigrationType _migrationType;

    public SqlServerSetup(AutoSetupContainerBuilder mainBuilder)
    {
        _mainBuilder = mainBuilder;
        // TODO move docker endpoint to a common class
        if(mainBuilder.DockerEndpoint != null)
            _msSqlBuilder = _msSqlBuilder.WithDockerEndpoint(mainBuilder.DockerEndpoint);
    }

    public AutoSetupContainerBuilder And()
    {
        return _mainBuilder;
    }

    public async Task<IContainer> BuildAndInitializeWithEfContextAsync<TContext>()
        where TContext : DbContext
    {
        var container = _msSqlBuilder.Build();
        await container.StartAsync();

        var connection = new DatabaseConnection
        {
            ConnectionString = BuildConnectionString(container.GetConnectionString()),
        };

        await ApplyEFCoreMigrationsAsync<TContext>(container, connection);
        // Here you would instantiate and run the orchestrator from our previous discussion
        // var schemaManager = new LiquibaseScriptManager(...);
        // var snapshotter = new MySqlSnapshotter();
        // var orchestrator = new DatabaseContainerOrchestrator(container, schemaManager, snapshotter, ...);
        // await orchestrator.InitializeAsync();

        return container;
    }

    public async Task<IContainer> BuildAndInitializeAsync()
    {
        var container = _msSqlBuilder.Build();
        await container.StartAsync();

        var connection = new DatabaseConnection
        {
            ConnectionString = BuildConnectionString(container.GetConnectionString()),
        };

        // Here you would instantiate and run the orchestrator from our previous discussion
        // var schemaManager = new LiquibaseScriptManager(...);
        // var snapshotter = new MySqlSnapshotter();
        // var orchestrator = new DatabaseContainerOrchestrator(container, schemaManager, snapshotter, ...);
        // await orchestrator.InitializeAsync();

        return container;
    }

    public IContainerSetup WithMigrationsPath(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"The migrations path does not exist: {fullPath}");
        }

        _migrationsPath = path;
        return this;
    }

    public SqlServerSetup UseDatabaseName(string dbName)
    {
        if(dbName.IsNullOrEmpty())
        {
            throw new InvalidParameterException("DB name cannot be null or empty.");
        }

        _dbName = dbName;
        return this;
    }

    public IEFCoreBuilder WithEFCoreMigrations()
    {
        _migrationType = MigrationType.EFCore;
        return this;
    }

    private async Task ApplyEFCoreMigrationsAsync<T>(IContainer container, DatabaseConnection connection)
        where T : DbContext
    {
        var manager = new EFCoreManager(_migrationsPath!, tryRecreateFromDump: false);
        await manager.InitializeDatabaseAsync<T>(container, connection, isLocalRun: false);
    }

    private string BuildConnectionString(string containerConnStr)
    {
        var connStrBuilder = new StringBuilder(containerConnStr);
        // TODO move to configs 
        connStrBuilder.Replace("localhost", "172.29.117.16");

        if(_dbName is not null)
        {
            connStrBuilder.Append(";Database=").Append(_dbName);            
        }

        return connStrBuilder.ToString();
    }
}