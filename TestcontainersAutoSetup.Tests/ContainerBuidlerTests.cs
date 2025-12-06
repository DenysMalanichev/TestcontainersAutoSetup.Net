using Microsoft.Data.SqlClient;
using DotNet.Testcontainers.Containers;
using Microsoft.eShopWeb.Infrastructure.Data;
using TestcontainersAutoSetup.Core.Implementation;
using TestcontainersAutoSetup.MySql.Implementation;
using TestcontainersAutoSetup.SqlServer.Implementation;
using Testcontainers.MsSql;
using System.Data.SqlTypes;
using System.Text;

namespace TestcontainersAutoSetup.Tests;

public class ContainerBuidlerTests
{
    // Feel free to change this variable to your own docker endpoint for now
    // TODO move to configs
    private const string wslDockerEndpoint = "tcp://localhost:2375";
    private readonly string? dockerEndpoint = CheckIfCiRun() ? null! : wslDockerEndpoint;

    [Fact]
    public async Task ContainerBuilder_CreatesMySqlContainer()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var mySqlContainer = await builder.CreateMySqlContainer()
            .WithDatabase("TestDb")
            .BuildAndInitializeAsync();

        Assert.NotEqual(mySqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, mySqlContainer.State);
    }

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainers()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var sqlServerContainer = await builder.CreateSqlServerContainer()
            .BuildAndInitializeAsync();

        Assert.NotEqual(default, sqlServerContainer.CreatedTime);
        Assert.Equal(TestcontainersStates.Running, sqlServerContainer.State);
    }

    [Fact]
    public async Task ContainerBuilder_CreatesBothSqlServerAndMySqlContainers()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var containers = await builder.CreateMySqlContainer()
            .WithDatabase("TestDb")
            .And()
            .CreateSqlServerContainer()
            .And()
            .BuildAsync();

        var sqlServerContainer = containers[0];
        var mySqlContainer = containers[1];

        Assert.NotEqual(mySqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, mySqlContainer.State);
        Assert.NotEqual(default, sqlServerContainer.CreatedTime);
        Assert.Equal(TestcontainersStates.Running, sqlServerContainer.State);
    }

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_ApplyingEFCoreMigrations()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .WithEFCoreMigrations()
            .BuildAndInitializeWithEfContextAsync<CatalogContext>();

        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        connectionString = connectionString.Replace("localhost", "172.29.117.16");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Verify Migrations
        using var historyCmd = new SqlCommand("SELECT COUNT(*) FROM __EFMigrationsHistory", connection);
        var migrationCount = (int)(await historyCmd.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.True(migrationCount > 0, "No migrations were found in the history table.");
    }

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_SeedingData()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .WithEFCoreMigrations()
            .BuildAndInitializeWithEfContextAsync<CatalogContext>();

        await Task.Delay(10_000);
        Assert.NotEqual(msSqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, msSqlContainer.State);

        // TODO move wsl2 ip replacement to the configs
        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        connectionString = connectionString.Replace("localhost", "172.29.117.16");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM CatalogTypes", connection);
        var count = (int)(await command.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.Equal(4, count);
    }

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_WithProperDatabase()
    {
        const string dbName = "CatalogTestDatabase";
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .UseDatabaseName(dbName)
            .WithEFCoreMigrations()
            // .WithMigrationsPath("../Migrations/MsSQL/EF Migrations")
            .BuildAndInitializeWithEfContextAsync<CatalogContext>();

        await Task.Delay(10_000);
        Assert.NotEqual(msSqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, msSqlContainer.State);
        
        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        var connStrBuilder = new StringBuilder(connectionString);
        // TODO move
        connStrBuilder.Replace("localhost", "172.29.117.16");
        connStrBuilder.Append(";Database=").Append(dbName);

        // Checking for migrations to ensure that db created
        using var connection = new SqlConnection(connStrBuilder.ToString());
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM CatalogTypes", connection);
        var count = (int)(await command.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.Equal(4, count);
    }

    private static bool CheckIfCiRun()
    {
        bool.TryParse(Environment.GetEnvironmentVariable("CI"), out bool env);
        if(env)
        {
            return true;
        }

        return false;
    }
}