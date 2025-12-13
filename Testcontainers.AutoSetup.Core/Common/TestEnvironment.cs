using DotNet.Testcontainers.Containers;
using Testcontainers.AutoSetup.Core.Abstractions;

namespace Testcontainers.AutoSetup.Core.Common;

public class TestEnvironment
{
    // A list of "tasks" to perform on reset
    private readonly List<(IContainer container, IDbStrategy strategy, string containerConnectionString)> _strategies = [];

    // The user registers containers they want us to manage
    public void Register<TContainer>(
        TContainer container, 
        IDbStrategy resetStrategy, 
        Func<TContainer, string> connectionStringProvider) 
        where TContainer : IContainer
    {
        _strategies.Add((container, resetStrategy, connectionStringProvider(container)));
    }

    // 1. Cold Start: Runs ONCE per project/session
    // Iterates all containers -> Seeds -> Snapshots
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        foreach (var strategy in _strategies)
        {
            await strategy.strategy.InitializeGlobalAsync(
                strategy.container,
                strategy.containerConnectionString,
                ct); 
        }
    }

    // 2. Warm Reset: Runs BEFORE each test
    // Iterates all containers -> Restores Snapshot/Dump
    public async Task ResetAsync(CancellationToken ct = default)
    {
        foreach (var strategy in _strategies)
        {
            await strategy.strategy.ResetAsync(
                strategy.container,
                strategy.containerConnectionString,
                ct);
        }
    }
}
