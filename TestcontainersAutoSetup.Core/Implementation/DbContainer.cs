using DotNet.Testcontainers.Containers;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common.Entities;

namespace TestcontainersAutoSetup.Core.Implementation;

public abstract class DbContainer : ContainerSetup, IDbContainer
{
    public List<DbSetup> DbSetups { get; set; } = new();
    public DbContainer(AutoSetupContainerBuilder mainBuilder) : base(mainBuilder)
    {
    }

    public abstract IDbContainer UseDatabaseName(string dbName);

    public IDbContainer AddDatabase(DbSetup dbSetup)
    {
        DbSetups.Add(dbSetup);

        return this;
    }

    protected DbSetup GetConfiguringDbSetup()
    {
        return DbSetups.Last();
    }
}