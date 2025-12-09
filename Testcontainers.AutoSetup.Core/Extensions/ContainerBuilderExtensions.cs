using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Testcontainers.AutoSetup.Core.Abstractions;

namespace Testcontainers.AutoSetup.Core.Extensions
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers a database seeder to be executed immediately after the container starts.
        /// </summary>
        /// <remarks>
        /// This method hooks into the container's lifecycle using <see cref="IContainerBuilder{TBuilder,TContainer,TConfiguration}.WithStartupCallback"/>.
        /// The provided <paramref name="seeder"/> logic runs after the container is in a "Running" state but before the control is returned to the test execution.
        /// This allows you to perform database migrations, execute scripts, or restore dumps automatically.
        /// </remarks>
        /// <typeparam name="TBuilder">The concrete type of the container builder (e.g., <c>MsSqlBuilder</c>, <c>PostgreSqlBuilder</c>).</typeparam>
        /// <typeparam name="TContainer">The concrete type of the container (e.g., <c>MsSqlContainer</c>, <c>PostgreSqlContainer</c>).</typeparam>
        /// <typeparam name="TConfiguration">The concrete type of the container configuration.</typeparam>
        /// <param name="builder">The container builder instance.</param>
        /// <param name="seeder">The implementation of <see cref="IDbSeeder"/> that contains the migration or seeding logic.</param>
        /// <param name="connectionStringProvider">
        /// A function that resolves the connection string from the running <typeparamref name="TContainer"/> instance. 
        /// <br/>Example: <c>c => c.GetConnectionString()</c>
        /// </param>
        /// <returns>The configured container builder instance.</returns>
        /// <example>
        /// <code>
        /// var container = new MsSqlBuilder()
        ///     .WithDbSeeder(new EfCoreMigrator&lt;MyDbContext&gt;(), c => c.GetConnectionString())
        ///     .Build();
        /// </code>
        /// </example>
        public static TBuilder WithDbSeeder<TBuilder, TContainer, TConfiguration>(
            this IContainerBuilder<TBuilder, TContainer, TConfiguration> builder,
            IDbSeeder seeder,
            Func<TContainer, string> connectionStringProvider)
                where TBuilder : IAbstractBuilder<TBuilder, TContainer, CreateContainerParameters>
                where TContainer : IContainer
                where TConfiguration : IContainerConfiguration
        {
            // This callback runs immediately after the container starts, but before your test gets control.
            return (TBuilder)builder.WithStartupCallback(async (container, ct) =>
            {
                // 1. Resolve the connection string using the user's provider
                var connectionString = connectionStringProvider(container);

                // 2. Run the seeding logic (EF Core, SQL Script, etc.)
                await seeder.SeedAsync(container, connectionString, ct);
            });
        }
    }
}