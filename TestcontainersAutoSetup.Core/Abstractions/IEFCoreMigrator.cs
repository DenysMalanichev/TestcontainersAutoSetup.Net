using Testcontainers.Core.Abstractions;

namespace TestcontainersAutoSetup.Core.Abstractions;

public interface IEFCoreMigrator
{
    IContainerSetup WithMigrationsPath(string path);
}