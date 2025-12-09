using DotNet.Testcontainers.Containers;
using Testcontainers.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common.Entities;

namespace TestcontainersAutoSetup.Core.Abstractions;

public interface IDbContainer<TContainer> : IContainerSetup<TContainer>
    where TContainer : IContainer
{
    /// <summary>
    /// Creates a DB within a container. May be used to create multiple DBs within one container
    /// </summary>
    IDbContainer<TContainer> AddDatabase(DbSetup dbSetup);

    /// <summary>
    /// Configures a container to create a database with provided name
    /// </summary>
    /// <param name="dbName"><see cref="string"/> DB name</param>
    /// <returns></returns>
    public IDbContainer<TContainer> UseDatabaseName(string dbName);

    /// <summary>
    /// Sets the MsSql password.
    /// </summary>
    /// <param name="password">The MsSql password.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    public IDbContainer<TContainer> WithPassword(string password);
}
