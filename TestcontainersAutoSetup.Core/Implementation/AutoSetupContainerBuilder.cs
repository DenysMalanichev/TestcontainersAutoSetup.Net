using DotNet.Testcontainers.Containers;
using TestcontainersAutoSetup.Core.Abstractions;

namespace TestcontainersAutoSetup.Core.Implementation;
public class AutoSetupContainerBuilder : AbstractAutoSetupContainerBuilder
{

    public AutoSetupContainerBuilder() : base()
    {
    }

    public AutoSetupContainerBuilder(string dockerEndpoint) : base(dockerEndpoint)
    {
    }

    public override async Task<List<IContainer>> BuildAsync()
    {
        var containers = new List<IContainer>();
        foreach (var setup in _containerSetups)
        {
            var container = await setup.BuildAndInitializeAsync();
            containers.Add(container);
        }
        return containers;
    }
}
