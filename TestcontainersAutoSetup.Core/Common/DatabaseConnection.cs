namespace TestcontainersAutoSetup.Core.Common;

public record DatabaseConnection
{
    public string ConnectionString { get; set; } = default!;
    public string ConnecrionPort { get; private set; } = default!;
    public bool IsLocalRun { get; private set; }
}