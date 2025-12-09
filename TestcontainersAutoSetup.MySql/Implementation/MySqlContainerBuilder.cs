using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.Core.Abstractions;
using Testcontainers.MySql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common;
using TestcontainersAutoSetup.Core.Implementation;

namespace TestcontainersAutoSetup.MySql.Implementation;
public class MySqlContainerBuilder : DbContainer<MySqlContainer>, IMigrationRunner
{
    private readonly AutoSetupContainerBuilder _mainBuilder;
    private readonly MySqlBuilder _mySqlBuilder = new MySqlBuilder();

    private string? _migrationsPath;
    private string? _snapshotDirectory;

    public MySqlContainerBuilder(AutoSetupContainerBuilder mainBuilder, IServiceProvider serviceProvider)
        : base(mainBuilder)
    {
        _mainBuilder = mainBuilder;
        if(mainBuilder.DockerEndpoint != null)
            _mySqlBuilder = _mySqlBuilder.WithDockerEndpoint(mainBuilder.DockerEndpoint);
    }

    public MySqlContainerBuilder WithMigrationsPath(string path)
    {
        _migrationsPath = path;
        return this;
    }

    public MySqlContainerBuilder WithPersistentSnapshotDirectory(string path)
    {
        _snapshotDirectory = path;
        _mySqlBuilder.WithBindMount(path, "/snapshots");
        return this;
    }

    public MySqlContainerBuilder WithDatabase(string db)
    {
        //_mySqlBuilder.WithDatabase(db);
        return this;
    }

    public AutoSetupContainerBuilder And()
    {
        return _mainBuilder;
    }

    // The build logic required by the interface
    public override async Task<IContainer> BuildAndInitializeAsync()
    {
        var container = _mySqlBuilder.Build();
        await container.StartAsync();

        // Here you would instantiate and run the orchestrator from our previous discussion
        // var schemaManager = new LiquibaseScriptManager(...);
        // var snapshotter = new MySqlSnapshotter();
        // var orchestrator = new DatabaseContainerOrchestrator(container, schemaManager, snapshotter, ...);
        // await orchestrator.InitializeAsync();

        return container;
    }

    public Task<IContainer> BuildAndInitializeWithEfContextAsync<T>() where T : DbContext
    {
        throw new NotImplementedException();
    }

    public Task ApplyEfCoreMigrationAsync<TContext>(IContainer container, DatabaseConnection connection) where TContext : DbContext
    {
        throw new NotImplementedException();
    }

    public override IDbContainer<MySqlContainer> UseDatabaseName(string dbName)
    {
        throw new NotImplementedException();
    }

    public override IDbContainer<MySqlContainer> WithPassword(string password)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithPort(int port, bool assignRandomHostPort = false)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithPort(string port, bool assignRandomHostPort = false)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithPort(int hostPort, int containerPort)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithPort(string hostPort, string containerPort)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> DependsOn(IContainer container)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> DependsOn(INetwork container)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> DependsOn(IVolume container, string description)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> DependsOn(IVolume container, string description, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithImage(string image)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithBindMount(string source, string description)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithBindMount(string source, string description, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithCleanUp(bool cleanUp)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithCommand(params string[] command)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithCommand(ComposableEnumerable<string> command)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithCreateParameterModifier(Action<CreateContainerParameters> command)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithEntrypoint(params string[] entrypoint)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithEnvironment(string name, string value)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithEnvironment(IReadOnlyDictionary<string, string> entrypoint)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithLabel(string name, string value)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithLabel(IReadOnlyDictionary<string, string> labels)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithExtraHost(string hostname, string ipAddress)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithHostname(string hostname)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithLogger(ILogger logger)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithMacAddress(string macaddress)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithMount(IMount mount)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithName(string name)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithNetwork(string name)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithNetwork(INetwork network)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithNetworkAliases(params string[] networkAliases)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithNetworkAliases(IEnumerable<string> networkAliases)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithOutputConsumer(IOutputConsumer outputConsumer)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithPrivileged(bool privileged)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(IResourceMapping resourceMapping)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithResourceMapping(Uri source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithStartupCallback(Func<MySqlContainer, CancellationToken, Task> startupCallback)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithTmpfsMount(string destination)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithTmpfsMount(string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithVolumeMount(string source, string destination)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithVolumeMount(IVolume volume, string destination)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithVolumeMount(IVolume volume, string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithVolumeMount(string volume, string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
        throw new NotImplementedException();
    }

    public override IContainerSetup<MySqlContainer> WithWorkingDirectory(string directory)
    {
        throw new NotImplementedException();
    }
}
