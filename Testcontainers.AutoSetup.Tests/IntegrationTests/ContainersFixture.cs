namespace Testcontainers.AutoSetup.Tests.IntegrationTests;

public class ContainersFixture : IAsyncLifetime
{
    public GlobalTestSetup Setup { get; } = new GlobalTestSetup();

    // CALLED ONCE: Before the first test in the collection runs
    public async Task InitializeAsync()
    {
        // This is the specific line you asked for
        await Setup.InitializeEnvironmentAsync();
    }

    // CALLED ONCE: After the last test in the collection finishes
    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            Setup.MsSqlContainerFromGenericBuilderConnection.DisposeAsync().AsTask(),
            Setup.MsSqlContainerFromSpecificBuilderConnection.DisposeAsync().AsTask()
        );
    }
}
