using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TestcontainersAutoSetup.Tests.Helpers;

public class DockerAddressHelper
{
    private static readonly string _dockerPort = TestConfig.GetConfiguration()["DockerPort"]!;
    private const string _gitHubActionsCiEnvVar = "CI";

    private static readonly Lazy<string> _dockerHostAddressLazy = new(GetDockerHostAddress);

    public static string DockerHostAddress
    {
        get { return _dockerHostAddressLazy.Value; }
    }

    /// <summary>
    /// Returns IP of Docker daemon in WSL2 or localhost Docker is not running in WSL2
    /// </summary>
    private static string GetDockerHostAddress()
    {
        // 1. If not on Windows, assume localhost (e.g., CI/CD or Linux host)
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "localhost";
        }

        try
        {
            // 2. Run the command "wsl hostname -I" to get the IP from inside the VM
            var processInfo = new ProcessStartInfo
            {
                FileName = "wsl",
                Arguments = "hostname -I",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null) return "localhost";

            // 3. Read the output (it might return multiple IPs, take the first one)
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var ip = output.Trim().Split(' ').FirstOrDefault();

            return string.IsNullOrWhiteSpace(ip) ? "localhost" : ip;
        }
        catch
        {
            // Fallback if wsl command fails or isn't installed
            return "localhost";
        }
    }

    /// <summary>
    /// Returns configured docker endpoint or null if the run is in CI
    /// </summary>
    public static string? GetDockerEndpoint()
    {
       if (IsCiRun())
        {
            return null;
        }

        return $"tcp://{DockerHostAddress}:{_dockerPort}";
    }

    private static bool IsCiRun()
    {
        var conversionResult = bool.TryParse(
            Environment.GetEnvironmentVariable(_gitHubActionsCiEnvVar), out bool env);
        if(conversionResult && env)
        {
            return true;
        }

        return false;
    }
}