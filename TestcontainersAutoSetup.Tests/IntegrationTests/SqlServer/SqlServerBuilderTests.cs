using Microsoft.Data.SqlClient;
using DotNet.Testcontainers.Containers;
using Microsoft.eShopWeb.Infrastructure.Data;
using TestcontainersAutoSetup.Core.Implementation;
using TestcontainersAutoSetup.SqlServer.Implementation;
using Testcontainers.MsSql;
using System.Data.SqlTypes;
using System.Text;
using TestcontainersAutoSetup.Tests.Helpers;
using TestcontainersAutoSetup.Core.Common.Entities.DbMigrationTypes;

namespace TestcontainersAutoSetup.Tests.IntegrationTests.SqlServer;

public class SqlServerBuilderTests
{
    private readonly string? dockerEndpoint = DockerAddressHelper.GetDockerEndpoint();

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_ApplyingEFCoreMigrations()
    {
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .AddDatabase(new EFCoreMigration<CatalogContext>())
            .BuildAndInitializeAsync();

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
            .AddDatabase(new EFCoreMigration<CatalogContext>())
            .BuildAndInitializeAsync();

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
            .AddDatabase(new EFCoreMigration<CatalogContext>())
            .UseDatabaseName(dbName)
            .BuildAndInitializeAsync();

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

    [Fact]
    public async Task ContainerBuilder_CreatesSqlServerContainer_WithMultipleDatabases()
    {
        const string dbName1 = "CatalogTestDatabase1";
        const string dbName2 = "CatalogTestDatabase2";
        var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
        var msSqlContainer = await builder.CreateSqlServerContainer()
            .AddDatabase(new EFCoreMigration<CatalogContext>())
            .UseDatabaseName(dbName1)
            .AddDatabase(new EFCoreMigration<CatalogContext>())
            .UseDatabaseName(dbName2)
            .BuildAndInitializeAsync();

        await Task.Delay(10_000);
        Assert.NotEqual(msSqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, msSqlContainer.State);

        string connectionString = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        var connStrBuilder = new StringBuilder(connectionString);
        connStrBuilder.Append(";Database=").Append(dbName1);

        // Checking for migrations to ensure that db created
        using var connection = new SqlConnection(connStrBuilder.ToString());
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM CatalogTypes", connection);
        var count = (int)(await command.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.Equal(4, count);

        string connectionString2 = ((MsSqlContainer)msSqlContainer).GetConnectionString();
        var connStrBuilder2 = new StringBuilder(connectionString);
        connStrBuilder2.Append(";Database=").Append(dbName1);

        // Checking for migrations to ensure that db created
        using var connection2 = new SqlConnection(connStrBuilder2.ToString());
        await connection2.OpenAsync();

        var count2 = (int)(await command.ExecuteScalarAsync() ?? throw new SqlNullValueException());

        Assert.Equal(4, count2);
    }
}