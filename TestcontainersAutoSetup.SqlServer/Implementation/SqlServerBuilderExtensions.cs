using TestcontainersAutoSetup.Core.Implementation;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public static class SqlServerContainerBuilderExtensions
{
    public static SqlServerContainerBuilder CreateSqlServerContainer(this AutoSetupContainerBuilder builder)
    {
        var sqlServerSetup = new SqlServerContainerBuilder(builder);

        builder.AddContainerSetup(sqlServerSetup);

        return sqlServerSetup;
    }
}
