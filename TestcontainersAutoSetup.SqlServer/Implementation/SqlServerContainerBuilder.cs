using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Testcontainers.Core.Abstractions;
using Testcontainers.MsSql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common;
using TestcontainersAutoSetup.Core.Enums;
using TestcontainersAutoSetup.Core.Implementation;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public class SqlServerContainerBuilder : DbContainer
{
    private readonly MsSqlBuilder _msSqlBuilder = new MsSqlBuilder();

    private string? _migrationsPath;
    private string? _snapshotDirectory;

    public SqlServerContainerBuilder(AutoSetupContainerBuilder mainBuilder) : base(mainBuilder)
    {
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

            await dbSetup.ExecuteAsync(container, connection);           
        }

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
}