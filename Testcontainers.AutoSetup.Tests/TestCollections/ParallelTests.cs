using Testcontainers.AutoSetup.Tests.IntegrationTests;

namespace Testcontainers.AutoSetup.Tests.TestCollections;

[CollectionDefinition(nameof(ParallelTests), DisableParallelization = false)]
public class ParallelTests : ICollectionFixture<ContainersFixture>
{
    // This class is just a marker for the collection definition
    // Parallelization ENABLED
}