using folders_sync;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

public class FolderSyncApp
{
    private static string _sourcePath;
    private static string _replicaPath;
    private static int _syncIntervalInMilliseconds;
    private static string _logFilePath;
    
    
    static async Task Main(string[] args)
    {
        if (!ParseArguments(args))
        {
            return;
        }
        
        // Override the log file path
        Environment.SetEnvironmentVariable("LogFilePath", _logFilePath);
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddNLog();
        });
        
        var logger = loggerFactory.CreateLogger<FolderSyncApp>();
        FolderSyncService folderSyncService = new FolderSyncService(_sourcePath, _replicaPath, logger);
        
        logger.LogInformation($"Starting folder synchronization from {_sourcePath} to {_replicaPath} every {_syncIntervalInMilliseconds / 1000} seconds.");

        while (true)
        {
            try
            {
                folderSyncService.Synchronize();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during synchronization: {ex.Message}");
            }

            await Task.Delay(_syncIntervalInMilliseconds);
        }


    }
    
    
    
    private static bool ParseArguments(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: SyncApp <source_path> <replica_path> <sync_interval_in_seconds> <log_file>");
            return false;
        }
        
        _sourcePath = Path.GetFullPath(args[0]);
        _replicaPath = Path.GetFullPath(args[1]);

        if (!Directory.Exists(_sourcePath))
        {
            Console.WriteLine($"Error: Source directory {_sourcePath} does not exist.");
            return false;
        }
        
        if (!Directory.Exists(_replicaPath))
        {
            Console.WriteLine($"Error: Replica directory {_replicaPath} does not exist.");
            return false;
        }
        
        if (!int.TryParse(args[2], out int syncIntervalInSeconds) || syncIntervalInSeconds <= 0)
        {
            Console.WriteLine($"Error: Invalid sync interval '{args[2]}'. Must be a value in seconds.");
            return false;
        }
        
        _syncIntervalInMilliseconds = syncIntervalInSeconds * 1000;
        _logFilePath = Path.GetFullPath(args[3]);
        
        // Get the parent directory of the log file
        string logDirectory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        return true;
    }
}