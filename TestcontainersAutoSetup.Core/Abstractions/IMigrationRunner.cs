using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using TestcontainersAutoSetup.Core.Common;

namespace TestcontainersAutoSetup.Core.Abstractions;
public interface IMigrationRunner
{
    Task ApplyEfCoreMigrationAsync<TContext>(IContainer container, DatabaseConnection connection) 
        where TContext : DbContext;
}