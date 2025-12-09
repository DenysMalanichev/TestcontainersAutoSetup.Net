using System.Text;
using DotNet.Testcontainers.Containers;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Enums;

namespace TestcontainersAutoSetup.Core.Common.Entities;

public abstract class DbSetup
{
    public MigrationType MigrationType { get; set; }
    public string? DbName { get; set; }
    public string? MigrationsPath { get; set; }
    public bool RestoreFromDump { get; set; } = false;

    public abstract Task ExecuteAsync(
        IMigrationRunner migrationRunner,
        IContainer container,
        DatabaseConnection connection);

    public string BuildConnectionString(string containerConnStr)
    {
        var connStrBuilder = new StringBuilder(containerConnStr);

        if(DbName is not null)
        {
            connStrBuilder.Append(";Database=").Append(DbName);            
        }

        return connStrBuilder.ToString();
    }
}