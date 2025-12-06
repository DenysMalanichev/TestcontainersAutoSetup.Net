using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using TestcontainersAutoSetup.Core.Implementation;

namespace Testcontainers.Core.Abstractions;

public interface IContainerSetup
{
    AutoSetupContainerBuilder And();
    Task<IContainer> BuildAndInitializeAsync();
    Task<IContainer> BuildAndInitializeWithEfContextAsync<T>() where T : DbContext;
}