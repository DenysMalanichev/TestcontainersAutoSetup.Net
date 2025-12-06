using System.Globalization;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TestcontainersAutoSetup.Core.Common;

namespace TestcontainersAutoSetup.SqlServer.Implementation
{
    // public class EFCoreManager
    // {
    //     private static readonly bool IsWindows = OperatingSystem.IsWindows();
    //     private const string SnapshotsDirectory = "Snapshots";
    //     private const string ScriptPath = @"../../../IntegrationTests/Scripts/";
    //     private const string LiquibasePath = @"../../../../../scripts/liquibase";

    //     public static async Task InitializeDatabaseAsync(
    //         IContainer container, DatabaseConnection connection, bool isLocalRun)
    //     {
    //         var latestSnapshotFile = await GetLatestSnapshotFileAsync(container).ConfigureAwait(false);

    //         if (!isLocalRun)
    //         {
    //             await RunLiquibaseUpdateAsync(connection).ConfigureAwait(false);
    //             return;
    //         }

    //         bool areScriptsUpdated = AreLiquibaseScriptsUpdated(latestSnapshotFile.LastWriteTime);

    //         if (latestSnapshotFile.Name is not null && areScriptsUpdated)
    //         {
    //             await RemoveDeprecatedDbSnapshotsAsync(container, latestSnapshotFile.Name!).ConfigureAwait(false);
    //         }

    //         if (latestSnapshotFile.Name is null || areScriptsUpdated)
    //         {
    //             await RunLiquibaseUpdateAsync(connection).ConfigureAwait(false);
    //             await CreateDatabaseStateSnapshotAsync(container, connection).ConfigureAwait(false);
    //         }
    //         else
    //         {
    //             await RestoreDatabaseStateAsync(container, latestSnapshotFile.Name).ConfigureAwait(false);
    //         }
    //     }

    //     private static async Task RunLiquibaseUpdateAsync(DatabaseConnection connection)
    //     {
    //         await DropDatabaseIfExistsAsync(connection.ConnectionString).ConfigureAwait(false);

    //         string file = ScriptPath + (IsWindows ? "update.bat" : "update.sh");

    //         ProcessStartInfo processInfo = new ProcessStartInfo
    //         {
    //             FileName = file,
    //             Arguments = connection.ConnecrionPort,
    //             RedirectStandardOutput = true,
    //             RedirectStandardError = true,
    //             UseShellExecute = false,
    //             CreateNoWindow = true
    //         };

    //         using Process process = new Process { StartInfo = processInfo };
    //         // Event handlers for real-time output logging
    //         process.OutputDataReceived += (sender, args) =>
    //         {
    //             if (!string.IsNullOrEmpty(args.Data))
    //             {
    //                 Console.WriteLine($"[STDOUT] {args.Data}");
    //             }
    //         };

    //         process.ErrorDataReceived += (sender, args) =>
    //         {
    //             if (!string.IsNullOrEmpty(args.Data))
    //             {
    //                 Console.WriteLine($"[STDERR] {args.Data}");
    //             }
    //         };

    //         process.Start();

    //         // Begin async reading
    //         process.BeginOutputReadLine();
    //         process.BeginErrorReadLine();

    //         await Task.Run(process.WaitForExit).ConfigureAwait(false);

    //         if (process.ExitCode != 0)
    //         {
    //             throw new InvalidOperationException($"Liquibase update failed with exit code {process.ExitCode}.");
    //         }
    //     }

    //     private static async Task DropDatabaseIfExistsAsync(string connectionString)
    //     {
    //         var builder = new MySqlConnectionStringBuilder(connectionString);

    //         using var connection = new MySqlConnection(builder.ConnectionString);
    //         await connection.OpenAsync().ConfigureAwait(false);
    //         using var command = connection.CreateCommand();
    //         command.CommandText = "DROP DATABASE IF EXISTS Catalog;";
    //         await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    //     }

    //     private static bool AreLiquibaseScriptsUpdated(DateTime? snapshotUpdateDate)
    //     {
    //         if (snapshotUpdateDate is null)
    //         {
    //             return true;
    //         }

    //         // Get the latest modification time of Liquibase scripts
    //         var lastModificationTime = Directory.GetFiles(ScriptPath, "*", SearchOption.AllDirectories)
    //             .Concat(Directory.GetFiles(LiquibasePath, "*", SearchOption.AllDirectories))
    //             .Max(File.GetLastWriteTime);

    //         return lastModificationTime > snapshotUpdateDate;
    //     }

    //     private static async Task RemoveDeprecatedDbSnapshotsAsync(IContainer container, string snapshotFileName)
    //     {
    //         var removeDumpCommand =
    //             $"rm -r {SnapshotsDirectory}/*";
    //         var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", removeDumpCommand }).ConfigureAwait(false);

    //         if (result.ExitCode != 0)
    //         {
    //             throw new InvalidOperationException($"Failed to remove a snapshot ({result.Stdout} {result.Stderr})");
    //         }
    //     }

    //     private static async Task CreateDatabaseStateSnapshotAsync(IContainer container, DatabaseConnection connectionData)
    //     {
    //         using var connection = new MySqlConnection(connectionData.ConnectionString);
    //         await connection.OpenAsync().ConfigureAwait(false);

    //         using var command = connection.CreateCommand();
    //         command.CommandText = "FLUSH TABLES WITH READ LOCK;";
    //         await command.ExecuteNonQueryAsync().ConfigureAwait(false);

    //         // Execute commands to export the database state to a file
    //         var snapshotFileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.sql";
    //         var createAndDumpCommand =
    //             $"mkdir -p {SnapshotsDirectory} && cd {SnapshotsDirectory} && mysqldump -u root --password=123456Ab Catalog --result-file={snapshotFileName}";
    //         var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", createAndDumpCommand }).ConfigureAwait(false);

    //         if (result.ExitCode != 0)
    //         {
    //             throw new InvalidOperationException($"Failed to create a snapshot ({result.Stdout} {result.Stderr})");
    //         }

    //         command.CommandText = "UNLOCK TABLES;";
    //         await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    //     }

    //     private static async Task RestoreDatabaseStateAsync(IContainer container, string snapshotFile)
    //     {
    //         // Execute commands to import the database state from the snapshot file
    //         var restoreCommand = $"cd {SnapshotsDirectory} && mysqladmin create Catalog ; mysql Catalog < {snapshotFile}";
    //         var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", restoreCommand }).ConfigureAwait(false);
    //         if (result.ExitCode != 0)
    //         {
    //             throw new InvalidOperationException($"Failed to load a dump: {result.Stdout} {result.Stderr}");
    //         }
    //     }

    //     private static async Task<(string? Name, DateTime? LastWriteTime)> GetLatestSnapshotFileAsync(IContainer container)
    //     {
    //         var result = await container.ExecAsync(
    //             new List<string> { "ls", "-t", SnapshotsDirectory, }
    //             ).ConfigureAwait(false);
    //         if (result.ExitCode != 0)
    //         {
    //             return (null!, null!);
    //         }
    //         var snapshots = result.Stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    //         var currentSnapshotName = snapshots.FirstOrDefault();
    //         if (currentSnapshotName is null)
    //         {
    //             return (null!, null!);
    //         }
    //         var snapshotTimeStamp = currentSnapshotName.Split('.')[0]!;
    //         if (!DateTime.TryParseExact(snapshotTimeStamp, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var lastWriteTime))
    //         {
    //             throw new InvalidOperationException($"Failed to parse snapshot timestamp {snapshotTimeStamp}");
    //         }
    //         return (currentSnapshotName, lastWriteTime);
    //     }
    // }
    public class EFCoreManager
    {
        private static readonly bool IsWindows = OperatingSystem.IsWindows();
        private const string SnapshotsDirectory = "Snapshots";
        private readonly string _migrationsPath;
        private readonly bool _tryRecreateFromDump;

        public EFCoreManager(string migrationsPath, bool tryRecreateFromDump = false)
        {
            _migrationsPath = migrationsPath;
            _tryRecreateFromDump = tryRecreateFromDump;
        }

        public async Task InitializeDatabaseAsync<TContext>(
            IContainer container, DatabaseConnection connection, bool isLocalRun)
                where TContext : DbContext
        {
            if(_tryRecreateFromDump)
            {
                await InitializeDatabaseWithDumpAsync(container, connection, isLocalRun);
            }

            await ApplyEFMigrationsAsync<TContext>(connection).ConfigureAwait(false);
        }

        private async Task InitializeDatabaseWithDumpAsync(IContainer container, DatabaseConnection connection, bool isLocalRun)
        {
            // var latestSnapshotFile = await GetLatestSnapshotFileAsync(container).ConfigureAwait(false);

            // if (!isLocalRun)
            // {
            //     await ApplyEFMigrationsAsync(connection).ConfigureAwait(false);
            //     return;
            // }

            // bool areScriptsUpdated = AreLiquibaseScriptsUpdated(latestSnapshotFile.LastWriteTime);

            // if (latestSnapshotFile.Name is not null && areScriptsUpdated)
            // {
            //     await RemoveDeprecatedDbSnapshotsAsync(container, latestSnapshotFile.Name!).ConfigureAwait(false);
            // }

            // if (latestSnapshotFile.Name is null || areScriptsUpdated)
            // {
            //     await ApplyEFMigrationsAsync(connection).ConfigureAwait(false);
            //     await CreateDatabaseStateSnapshotAsync(container, connection).ConfigureAwait(false);
            // }
            // else
            // {
            //     await RestoreDatabaseStateAsync(container, latestSnapshotFile.Name).ConfigureAwait(false);
            // }
        }

        private async Task ApplyEFMigrationsAsync<TContext>(DatabaseConnection connection)
            where TContext : DbContext
        {
            Console.WriteLine($"[Migration] Preparing to migrate {typeof(TContext).Name}...");

            var dbContext = InstantiateDbContext<TContext>(connection);

            await using (dbContext)
            {
                Console.WriteLine($"[Migration] Applying pending migrations for {typeof(TContext).Name}...");
                await dbContext.Database.MigrateAsync();
                Console.WriteLine($"[Migration] Done.");
            }
        }

        private TContext InstantiateDbContext<TContext>(DatabaseConnection connection) where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder
                .UseSqlServer(connection.ConnectionString)
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));;

            TContext dbContext;
            try
            {
                dbContext = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"The DbContext '{typeof(TContext).Name}' does not have a constructor that accepts DbContextOptions<T>. " +
                    "Ensure your Context class looks like: 'public MyContext(DbContextOptions<MyContext> options) : base(options) { }'");
            }

            return dbContext;
        }

        private static async Task DropDatabaseIfExistsAsync(string connectionString)
        {
            // var builder = new MySqlConnectionStringBuilder(connectionString);

            // using var connection = new MySqlConnection(builder.ConnectionString);
            // await connection.OpenAsync().ConfigureAwait(false);
            // using var command = connection.CreateCommand();
            // command.CommandText = "DROP DATABASE IF EXISTS Catalog;";
            // await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        // private static bool AreLiquibaseScriptsUpdated(DateTime? snapshotUpdateDate)
        // {
        //     if (snapshotUpdateDate is null)
        //     {
        //         return true;
        //     }

        //     // Get the latest modification time of Liquibase scripts
        //     var lastModificationTime = Directory.GetFiles(MigrationsPath, "*", SearchOption.AllDirectories)
        //         .Concat(Directory.GetFiles(LiquibasePath, "*", SearchOption.AllDirectories))
        //         .Max(File.GetLastWriteTime);

        //     return lastModificationTime > snapshotUpdateDate;
        // }

        // private static async Task RemoveDeprecatedDbSnapshotsAsync(IContainer container, string snapshotFileName)
        // {
        //     var removeDumpCommand =
        //         $"rm -r {SnapshotsDirectory}/*";
        //     var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", removeDumpCommand }).ConfigureAwait(false);

        //     if (result.ExitCode != 0)
        //     {
        //         throw new InvalidOperationException($"Failed to remove a snapshot ({result.Stdout} {result.Stderr})");
        //     }
        // }

        // private static async Task CreateDatabaseStateSnapshotAsync(IContainer container, DatabaseConnection connectionData)
        // {
        //     using var connection = new MySqlConnection(connectionData.ConnectionString);
        //     await connection.OpenAsync().ConfigureAwait(false);

        //     using var command = connection.CreateCommand();
        //     command.CommandText = "FLUSH TABLES WITH READ LOCK;";
        //     await command.ExecuteNonQueryAsync().ConfigureAwait(false);

        //     // Execute commands to export the database state to a file
        //     var snapshotFileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.sql";
        //     var createAndDumpCommand =
        //         $"mkdir -p {SnapshotsDirectory} && cd {SnapshotsDirectory} && mysqldump -u root --password=123456Ab Catalog --result-file={snapshotFileName}";
        //     var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", createAndDumpCommand }).ConfigureAwait(false);

        //     if (result.ExitCode != 0)
        //     {
        //         throw new InvalidOperationException($"Failed to create a snapshot ({result.Stdout} {result.Stderr})");
        //     }

        //     command.CommandText = "UNLOCK TABLES;";
        //     await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        // }

        private static async Task RestoreDatabaseStateAsync(IContainer container, string snapshotFile)
        {
            // Execute commands to import the database state from the snapshot file
            var restoreCommand = $"cd {SnapshotsDirectory} && mysqladmin create Catalog ; mysql Catalog < {snapshotFile}";
            var result = await container.ExecAsync(new List<string> { "/bin/bash", "-c", restoreCommand }).ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException($"Failed to load a dump: {result.Stdout} {result.Stderr}");
            }
        }

        private async Task<(string? Name, DateTime? LastWriteTime)> GetLatestSnapshotFileAsync(IContainer container)
        {
            var result = await container.ExecAsync(
                new List<string> { "ls", "-t", SnapshotsDirectory, }
                ).ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return (null!, null!);
            }
            var snapshots = result.Stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var currentSnapshotName = snapshots.FirstOrDefault();
            if (currentSnapshotName is null)
            {
                return (null!, null!);
            }
            var snapshotTimeStamp = currentSnapshotName.Split('.')[0]!;
            if (!DateTime.TryParseExact(snapshotTimeStamp, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var lastWriteTime))
            {
                throw new InvalidOperationException($"Failed to parse snapshot timestamp {snapshotTimeStamp}");
            }
            return (currentSnapshotName, lastWriteTime);
        }
    }
}