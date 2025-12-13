using Testcontainers.AutoSetup.Core.Common.Enums;

namespace Testcontainers.AutoSetup.Core.Common.Entities;

public record DbSetup
{   
    public DbType DbType { get; set; } = DbType.Other;
    public string? DbName { get; set; }
    public string? MigrationsPath { get; set; }
    public bool RestoreFromDump { get; set; } = false;

    public string BuildConnectionString(string containerConnStr)
    {
        if(DbName is not null)
        {
            containerConnStr = containerConnStr.Replace("Database=master", $"Database={DbName}");            
        }

        containerConnStr += ";Encrypt=False";

        return containerConnStr;
    }
}