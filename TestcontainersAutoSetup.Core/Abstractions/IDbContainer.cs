using Testcontainers.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common.Entities;

namespace TestcontainersAutoSetup.Core.Abstractions;

public interface IDbContainer : IContainerSetup
{
    /// <summary>
    /// Creates a DB within a container. May be used to create multiple DBs within one container
    /// </summary>
    IDbContainer AddDatabase(DbSetup dbSetup);

    public IDbContainer UseDatabaseName(string dbName);

    public IDbContainer WithMigrationsPath(string path);
}
