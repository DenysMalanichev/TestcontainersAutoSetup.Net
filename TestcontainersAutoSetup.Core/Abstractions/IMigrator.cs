using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using TestcontainersAutoSetup.Core.Common;

namespace TestcontainersAutoSetup.Core.Abstractions;
public interface IMigrator
{
    Task ApplyEFCoreMigrationsAsync<TContext>(IContainer container, DatabaseConnection connection) 
        where TContext : DbContext;
}