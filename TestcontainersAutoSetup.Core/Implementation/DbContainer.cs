using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.Core.Abstractions;
using TestcontainersAutoSetup.Core.Abstractions;
using TestcontainersAutoSetup.Core.Common.Entities;

namespace TestcontainersAutoSetup.Core.Implementation;

public abstract class DbContainer<TContainer> : ContainerSetup, IDbContainer<TContainer>
    where TContainer : IContainer
{
    public List<DbSetup> DbSetups { get; set; } = new();
    public DbContainer(AutoSetupContainerBuilder mainBuilder) : base(mainBuilder)
    {
    }

    public abstract IDbContainer<TContainer> UseDatabaseName(string dbName);

    public IDbContainer<TContainer> AddDatabase(DbSetup dbSetup)
    {
        DbSetups.Add(dbSetup);

        return this;
    }

    protected DbSetup GetConfiguringDbSetup()
    {
        if(DbSetups.IsNullOrEmpty())
        {
            throw new ArgumentNullException("No database created within the container yet.");
        }

        return DbSetups.Last();
    }

    public IDbContainer<TContainer> WithMigrationsPath(string path)
    {
        var dbSetup = GetConfiguringDbSetup();
        
        if(dbSetup.MigrationType == Enums.MigrationType.EFCore)
        {
            // TODO LOG warning as no path is needed for EF
            return this; 
        }

        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"The migrations path does not exist: {fullPath}");
        }

        if(dbSetup.MigrationsPath.IsNullOrEmpty())
        {
            dbSetup.MigrationsPath = path;
        }

        return this;
    }

    public IDbContainer<TContainer> RestoreFromDump(bool restoreFromDump = true)
    {
        var dbSetup = GetConfiguringDbSetup();
        dbSetup.RestoreFromDump = restoreFromDump;

        return this;
    }

    public abstract IDbContainer<TContainer> WithPassword(string password);

    public abstract IContainerSetup<TContainer> WithPort(int port, bool assignRandomHostPort = false);

    public abstract IContainerSetup<TContainer> WithPort(string port, bool assignRandomHostPort = false);

    public abstract IContainerSetup<TContainer> WithPort(int hostPort, int containerPort);

    public abstract IContainerSetup<TContainer> WithPort(string hostPort, string containerPort);

    public abstract IContainerSetup<TContainer> DependsOn(IContainer container);

    public abstract IContainerSetup<TContainer> DependsOn(INetwork container);

    public abstract IContainerSetup<TContainer> DependsOn(IVolume container, string description);

    public abstract IContainerSetup<TContainer> DependsOn(IVolume container, string description, AccessMode accessMode);

    public abstract IContainerSetup<TContainer> WithImage(string image);

    public abstract IContainerSetup<TContainer> WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy);

    public abstract IContainerSetup<TContainer> WithAcceptLicenseAgreement(bool acceptLicenseAgreement);

    public abstract IContainerSetup<TContainer> WithBindMount(string source, string description);

    public abstract IContainerSetup<TContainer> WithBindMount(string source, string description, AccessMode accessMode);

    public abstract IContainerSetup<TContainer> WithCleanUp(bool cleanUp);

    IContainerSetup<TContainer> IContainerSetup<TContainer>.RestoreFromDump(bool RestoreFromDump)
    {
        return this.RestoreFromDump(RestoreFromDump);
    }

    IContainerSetup<TContainer> IContainerSetup<TContainer>.WithMigrationsPath(string path)
    {
        return WithMigrationsPath(path);
    }

    public abstract IContainerSetup<TContainer> WithCommand(params string[] command);

    public abstract IContainerSetup<TContainer> WithCommand(ComposableEnumerable<string> command);

    public abstract IContainerSetup<TContainer> WithCreateParameterModifier(Action<CreateContainerParameters> command);

    public abstract IContainerSetup<TContainer> WithEntrypoint(params string[] entrypoint);

    public abstract IContainerSetup<TContainer> WithEnvironment(string name, string value);

    public abstract IContainerSetup<TContainer> WithEnvironment(IReadOnlyDictionary<string, string> entrypoint);

    public abstract IContainerSetup<TContainer> WithLabel(string name, string value);

    public abstract IContainerSetup<TContainer> WithLabel(IReadOnlyDictionary<string, string> labels);

    public abstract IContainerSetup<TContainer> WithExtraHost(string hostname, string ipAddress);

    public abstract IContainerSetup<TContainer> WithHostname(string hostname);

    public abstract IContainerSetup<TContainer> WithLogger(ILogger logger);

    public abstract IContainerSetup<TContainer> WithMacAddress(string macaddress);

    public abstract IContainerSetup<TContainer> WithMount(IMount mount);

    public abstract IContainerSetup<TContainer> WithName(string name);

    public abstract IContainerSetup<TContainer> WithNetwork(string name);

    public abstract IContainerSetup<TContainer> WithNetwork(INetwork network);

    public abstract IContainerSetup<TContainer> WithNetworkAliases(params string[] networkAliases);

    public abstract IContainerSetup<TContainer> WithNetworkAliases(IEnumerable<string> networkAliases);

    public abstract IContainerSetup<TContainer> WithOutputConsumer(IOutputConsumer outputConsumer);

    public abstract IContainerSetup<TContainer> WithPrivileged(bool privileged);

    public abstract IContainerSetup<TContainer> WithResourceMapping(IResourceMapping resourceMapping);

    public abstract IContainerSetup<TContainer> WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithResourceMapping(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithResourceMapping(Uri source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public abstract IContainerSetup<TContainer> WithStartupCallback(Func<TContainer, CancellationToken, Task> startupCallback);

    public abstract IContainerSetup<TContainer> WithTmpfsMount(string destination);

    public abstract IContainerSetup<TContainer> WithTmpfsMount(string destination, AccessMode accessMode);

    public abstract IContainerSetup<TContainer> WithVolumeMount(string source, string destination);

    public abstract IContainerSetup<TContainer> WithVolumeMount(IVolume volume, string destination);

    public abstract IContainerSetup<TContainer> WithVolumeMount(IVolume volume, string destination, AccessMode accessMode);

    public abstract IContainerSetup<TContainer> WithVolumeMount(string volume, string destination, AccessMode accessMode);

    public abstract IContainerSetup<TContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy);

    public abstract IContainerSetup<TContainer> WithWorkingDirectory(string directory);
}