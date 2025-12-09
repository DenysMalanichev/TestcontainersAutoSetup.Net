using DotNet.Testcontainers.Containers;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;
using Microsoft.Extensions.Logging;
using TestcontainersAutoSetup.Core.Implementation;

namespace Testcontainers.Core.Abstractions;

public interface IContainerSetup<TContainer> : IContainerSetup
    where TContainer : IContainer
{
    public IContainerSetup<TContainer> WithPort(int port, bool assignRandomHostPort = false);
    public IContainerSetup<TContainer> WithPort(string port, bool assignRandomHostPort = false);
    public IContainerSetup<TContainer> WithPort(int hostPort, int containerPort);
    public IContainerSetup<TContainer> WithPort(string hostPort, string containerPort);

    public IContainerSetup<TContainer> DependsOn(IContainer container);
    public IContainerSetup<TContainer> DependsOn(INetwork container);
    public IContainerSetup<TContainer> DependsOn(IVolume container, string description);
    public IContainerSetup<TContainer> DependsOn(IVolume container, string description, AccessMode accessMode);

    public IContainerSetup<TContainer> WithImage(string image);
    public IContainerSetup<TContainer> WithImagePullPolicy(Func<ImageInspectResponse, bool> imagePullPolicy);

    public IContainerSetup<TContainer> WithAcceptLicenseAgreement(bool acceptLicenseAgreement);

    public IContainerSetup<TContainer> WithBindMount(string source, string description);
    public IContainerSetup<TContainer> WithBindMount(string source, string description, AccessMode accessMode);

    public IContainerSetup<TContainer> WithCleanUp(bool cleanUp);

    public IContainerSetup<TContainer> RestoreFromDump(bool RestoreFromDump = true);

    public IContainerSetup<TContainer> WithMigrationsPath(string path);

    public IContainerSetup<TContainer> WithCommand(params string[] command);
    public IContainerSetup<TContainer> WithCommand(ComposableEnumerable<string> command);


    public IContainerSetup<TContainer> WithCreateParameterModifier(Action<CreateContainerParameters> command);
    public IContainerSetup<TContainer> WithEntrypoint(params string[] entrypoint);

    public IContainerSetup<TContainer> WithEnvironment(string name, string value);
    public IContainerSetup<TContainer> WithEnvironment(IReadOnlyDictionary<string, string> entrypoint);

    public IContainerSetup<TContainer> WithLabel(string name, string value);
    public IContainerSetup<TContainer> WithLabel(IReadOnlyDictionary<string, string> labels);

    public IContainerSetup<TContainer> WithExtraHost(string hostname, string ipAddress);
    public IContainerSetup<TContainer> WithHostname(string hostname);

    public IContainerSetup<TContainer> WithLogger(ILogger logger);

    public IContainerSetup<TContainer> WithMacAddress(string macaddress);

    public IContainerSetup<TContainer> WithMount(IMount mount);

    public IContainerSetup<TContainer> WithName(string name);

    public IContainerSetup<TContainer> WithNetwork(string name);
    public IContainerSetup<TContainer> WithNetwork(INetwork network);
    public IContainerSetup<TContainer> WithNetworkAliases(params string[] networkAliases);
    public IContainerSetup<TContainer> WithNetworkAliases(IEnumerable<string> networkAliases);

    public IContainerSetup<TContainer> WithOutputConsumer(IOutputConsumer outputConsumer);    
    public IContainerSetup<TContainer> WithPrivileged(bool privileged);

    public IContainerSetup<TContainer> WithResourceMapping(IResourceMapping resourceMapping);
    public IContainerSetup<TContainer> WithResourceMapping(byte[] resourceContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);
    public IContainerSetup<TContainer> WithResourceMapping(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);
    public IContainerSetup<TContainer> WithResourceMapping(FileInfo source, FileInfo target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);
    public IContainerSetup<TContainer> WithResourceMapping(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);
    public IContainerSetup<TContainer> WithResourceMapping(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);
    public IContainerSetup<TContainer> WithResourceMapping(Uri source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead);

    public IContainerSetup<TContainer> WithStartupCallback(Func<TContainer, CancellationToken, Task> startupCallback);  

    public IContainerSetup<TContainer> WithTmpfsMount(string destination);
    public IContainerSetup<TContainer> WithTmpfsMount(string destination, AccessMode accessMode);

    public IContainerSetup<TContainer> WithVolumeMount(string source, string destination);
    public IContainerSetup<TContainer> WithVolumeMount(IVolume volume, string destination);
    public IContainerSetup<TContainer> WithVolumeMount(IVolume volume, string destination, AccessMode accessMode);
    public IContainerSetup<TContainer> WithVolumeMount(string volume, string destination, AccessMode accessMode);

    public IContainerSetup<TContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy);
    public IContainerSetup<TContainer> WithWorkingDirectory(string directory);
}

public interface IContainerSetup
{
    AutoSetupContainerBuilder And();
    Task<IContainer> BuildAndInitializeAsync();
}