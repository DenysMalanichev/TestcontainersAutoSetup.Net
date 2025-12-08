using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using TestcontainersAutoSetup.Core.Implementation;
using TestcontainersAutoSetup.MySql.Implementation;
using TestcontainersAutoSetup.SqlServer.Implementation;
using TestcontainersAutoSetup.Tests.Helpers;

namespace TestcontainersAutoSetup.Tests.IntegrationTests.Core;

public class ContainerBuidlerTests
{
    private readonly string? dockerEndpoint = DockerAddressHelper.GetDockerEndpoint();
    private readonly IServiceProvider emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

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
        var sqlServerContainer = await builder.CreateSqlServerContainer(emptyServiceProvider)
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
            .CreateSqlServerContainer(emptyServiceProvider)
            .And()
            .BuildAsync();

        var sqlServerContainer = containers[0];
        var mySqlContainer = containers[1];

        Assert.NotEqual(mySqlContainer.CreatedTime, default);
        Assert.Equal(TestcontainersStates.Running, mySqlContainer.State);
        Assert.NotEqual(default, sqlServerContainer.CreatedTime);
        Assert.Equal(TestcontainersStates.Running, sqlServerContainer.State);
    }
}