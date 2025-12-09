using Testcontainers.MySql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Implementation;
using TestcontainersAutoSetup.MySql.Implementation;

namespace TestcontainersAutoSetup.MySql.Implementation;

public static class MySqlBuilderExtensions
{
    public static MySqlContainerBuilder CreateMySqlContainer(this AutoSetupContainerBuilder builder,
        IServiceProvider serviceProvider)
    {
        var mySqlSetup = new MySqlContainerBuilder(builder, serviceProvider);

        builder.AddContainerSetup<MySqlContainer>(mySqlSetup);

        return mySqlSetup;
    }
}
