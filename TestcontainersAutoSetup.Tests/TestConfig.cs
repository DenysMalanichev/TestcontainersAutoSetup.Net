using Microsoft.Extensions.Configuration;

namespace TestcontainersAutoSetup.Tests;

public static class TestConfig
{
    public static IConfigurationRoot GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables() 
            .Build();
    }
}