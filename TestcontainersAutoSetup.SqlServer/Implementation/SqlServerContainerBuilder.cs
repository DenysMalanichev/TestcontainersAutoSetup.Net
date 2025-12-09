using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Testcontainers.MsSql;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common;
using TestcontainersAutoSetup.Core.Implementation;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace TestcontainersAutoSetup.SqlServer.Implementation;

public class SqlServerContainerBuilder : DbContainer<MsSqlContainer>, IMigrationRunner
{
    private readonly IServiceProvider _serviceProvider;
    private MsSqlBuilder _msSqlBuilder = new();

    private string? _migrationsPath;
    private string? _snapshotDirectory;

    public SqlServerContainerBuilder(AutoSetupContainerBuilder mainBuilder, IServiceProvider serviceProvider)
        : base(mainBuilder)
    {
        _serviceProvider = serviceProvider;
        if(mainBuilder.DockerEndpoint != null)
            _msSqlBuilder = _msSqlBuilder.WithDockerEndpoint(mainBuilder.DockerEndpoint);
    }

    public override async Task<IContainer> BuildAndInitializeAsync()
    {
        var container = _msSqlBuilder.Build();
        await container.StartAsync();

        foreach (var dbSetup in DbSetups)
        {
            var connection = new DatabaseConnection
            {
                ConnectionString = dbSetup.BuildConnectionString(container.GetConnectionString()),
            };

            await dbSetup.ExecuteAsync(this, container, connection);           
        }

        return container;
    }

    public override SqlServerContainerBuilder UseDatabaseName(string dbName)
    {
        if(dbName.IsNullOrEmpty())
        {
            throw new InvalidParameterException("DB name cannot be null or empty.");
        }

        var dbSetup = GetConfiguringDbSetup();
        dbSetup.DbName = dbName;
        return this;
    }

    public async Task ApplyEfCoreMigrationAsync<TContext>(IContainer container, DatabaseConnection connection)
        where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseSqlServer(connection.ConnectionString);

        using var scope = _serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var context = ActivatorUtilities.CreateInstance<TContext>(
            scopedProvider, 
            optionsBuilder.Options
        );

        await context.Database.MigrateAsync();
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithPassword(string password)
    {
        _msSqlBuilder = _msSqlBuilder.WithPassword(password);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithImage(string image)
    {
        _msSqlBuilder = _msSqlBuilder.WithImage(image);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithPort(int port, bool assignRandomHostPort = false)
    {
        _msSqlBuilder = _msSqlBuilder.WithPortBinding(port, assignRandomHostPort);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithPort(string port, bool assignRandomHostPort = false)
    {
        _msSqlBuilder = _msSqlBuilder.WithPortBinding(port, assignRandomHostPort);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithPort(int hostPort, int containerPort)
    {
        _msSqlBuilder = _msSqlBuilder.WithPortBinding(hostPort, containerPort);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithPort(string hostPort, string containerPort)
    {
        _msSqlBuilder = _msSqlBuilder.WithPortBinding(hostPort, containerPort);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder DependsOn(IContainer container)
    {
        _msSqlBuilder = _msSqlBuilder.DependsOn(container);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder DependsOn(INetwork network)
    {
        _msSqlBuilder = _msSqlBuilder.DependsOn(network);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder DependsOn(IVolume container, string description)
    {
        _msSqlBuilder = _msSqlBuilder.DependsOn(container, description);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder DependsOn(IVolume container, string description, AccessMode accessMode)
    {
        _msSqlBuilder = _msSqlBuilder.DependsOn(container, description, accessMode);
        return this;
    }

    /// <inheritdoc/>
    public override SqlServerContainerBuilder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        _msSqlBuilder = _msSqlBuilder.WithAcceptLicenseAgreement(acceptLicenseAgreement);
        return this;    
    }

    public override SqlServerContainerBuilder WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithBindMount(string source, string description)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithBindMount(string source, string description, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithCleanUp(bool cleanUp)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithCommand(params string[] command)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithCommand(ComposableEnumerable<string> command)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithCreateParameterModifier(Action<CreateContainerParameters> command)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithEntrypoint(params string[] entrypoint)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithEnvironment(string name, string value)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithEnvironment(IReadOnlyDictionary<string, string> entrypoint)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithLabel(string name, string value)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithLabel(IReadOnlyDictionary<string, string> labels)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithExtraHost(string hostname, string ipAddress)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithHostname(string hostname)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithLogger(ILogger logger)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithMacAddress(string macaddress)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithMount(IMount mount)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithName(string name)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithNetwork(string name)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithNetwork(INetwork network)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithNetworkAliases(params string[] networkAliases)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithNetworkAliases(IEnumerable<string> networkAliases)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithOutputConsumer(IOutputConsumer outputConsumer)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithPrivileged(bool privileged)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(IResourceMapping resourceMapping)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithResourceMapping(Uri source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithStartupCallback(Func<MsSqlContainer, CancellationToken, Task> startupCallback)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithTmpfsMount(string destination)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithTmpfsMount(string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithVolumeMount(string source, string destination)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithVolumeMount(IVolume volume, string destination)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithVolumeMount(IVolume volume, string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithVolumeMount(string volume, string destination, AccessMode accessMode)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
        throw new NotImplementedException();
    }

    public override SqlServerContainerBuilder WithWorkingDirectory(string directory)
    {
        throw new NotImplementedException();
    }
}