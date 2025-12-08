using Microsoft.IdentityModel.Tokens;
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
        if(DbSetups.IsNullOrEmpty())
        {
            throw new ArgumentNullException("No database created within the container yet.");
        }

        return DbSetups.Last();
    }

    public IDbContainer WithMigrationsPath(string path)
    {
        var dbSetup = GetConfiguringDbSetup();
        
        if(dbSetup.MigrationType == Enums.MigrationType.EFCore)
        {
            // TODO LOG warning as no path is needed for EF
            return this; 
        }

        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"The migrations path does not exist: {fullPath}");
        }

        if(dbSetup.MigrationsPath.IsNullOrEmpty())
        {
            dbSetup.MigrationsPath = path;
        }

        return this;
    }
}