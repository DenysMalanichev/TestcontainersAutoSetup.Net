using Microsoft.Data.SqlClient;
using DotNet.Testcontainers.Containers;
using Microsoft.eShopWeb.Infrastructure.Data;
using TestcontainersAutoSetup.Core.Implementation;
using TestcontainersAutoSetup.SqlServer.Implementation;
using Testcontainers.MsSql;
using System.Data.SqlTypes;
using System.Text;
using TestcontainersAutoSetup.Tests.Helpers;

namespace TestcontainersAutoSetup.Tests.IntegrationTests.SqlServer;

public class SqlServerBuilderTests
{
    private readonly string? dockerEndpoint = DockerAddressHelper.GetDockerEndpoint();

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_ApplyingEFCoreMigrations()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .WithEFCoreMigrations()
            .BuildAndInitializeWithEfContextAsync<CatalogContext>();

        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();

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

        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();

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
            .BuildAndInitializeWithEfContextAsync<CatalogContext>();

        await Task.Delay(10_000);
        Assert.NotEqual(msSqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, msSqlContainer.State);

        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        var connStrBuilder = new StringBuilder(connectionString);
        connStrBuilder.Append(";Database=").Append(dbName);

        // Checking for migrations to ensure that db created
        using var connection = new SqlConnection(connStrBuilder.ToString());
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM CatalogTypes", connection);
        var count = (int)(await command.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.Equal(4, count);
    }
}