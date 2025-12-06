using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;

namespace TestcontainersAutoSetup.Core.Abstractions;

public interface IEFCoreBuilder
{
    Task<IContainer> BuildAndInitializeWithEfContextAsync<TContext>()
        where TContext : DbContext;
}