using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Testcontainers.Core.Abstractions;

namespace TestcontainersAutoSetup.Core.Implementation;

public abstract class ContainerSetup : IContainerSetup
{
    private readonly AutoSetupContainerBuilder _mainBuilder;

    public ContainerSetup(AutoSetupContainerBuilder mainBuilder)
    {
        _mainBuilder = mainBuilder;
    }

    public AutoSetupContainerBuilder And()
    {
        return _mainBuilder;
    }

    public abstract Task<IContainer> BuildAndInitializeAsync();
}