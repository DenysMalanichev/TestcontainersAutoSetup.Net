using DotNet.Testcontainers.Containers;
using Testcontainers.Core.Abstractions;

namespace TestcontainersAutoSetup.Core.Abstractions;

public abstract partial class AbstractAutoSetupContainerBuilder
{
    protected readonly List<IContainerSetup> _containerSetups = new();
    public string DockerEndpoint { get; private set; }

    public AbstractAutoSetupContainerBuilder(string dockerEndpoint)
    {
        DockerEndpoint = dockerEndpoint;   
    }

    public AbstractAutoSetupContainerBuilder() : this(string.Empty)
    {
    }

    public void AddContainerSetup<TContainer>(IContainerSetup<TContainer> setup)
        where TContainer : IContainer
    {
        _containerSetups.Add(setup);
    }

    public abstract Task<List<IContainer>> BuildAsync();
}