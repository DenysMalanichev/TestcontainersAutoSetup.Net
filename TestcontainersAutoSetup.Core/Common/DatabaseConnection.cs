namespace TestcontainersAutoSetup.Core.Common;

public record DatabaseConnection
{
    public string ConnectionString { get; set; } = default!;
    public string ConnecrionPort { get; set; } = default!;
}