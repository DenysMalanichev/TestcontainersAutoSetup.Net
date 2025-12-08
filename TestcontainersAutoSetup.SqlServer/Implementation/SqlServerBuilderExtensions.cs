using TestcontainersAutoSetup.Core.Implementation;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public static class SqlServerContainerBuilderExtensions
{
    public static SqlServerContainerBuilder CreateSqlServerContainer(this AutoSetupContainerBuilder builder,
        IServiceProvider serviceProvider)
    {
        var sqlServerSetup = new SqlServerContainerBuilder(builder, serviceProvider);

        builder.AddContainerSetup(sqlServerSetup);

        return sqlServerSetup;
    }
}
