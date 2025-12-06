
namespace TestcontainersAutoSetup.Core.Abstractions;
public interface IMigrator
{
    IEFCoreBuilder WithEFCoreMigrations();
}