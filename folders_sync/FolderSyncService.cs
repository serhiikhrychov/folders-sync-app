namespace folders_sync;
using Microsoft.Extensions.Logging;

public class FolderSyncService
{
    private readonly string _sourcePath;
    private readonly string _replicaPath;
    private readonly ILogger _logger;

    public FolderSyncService(string sourcePath, string replicaPath, ILogger logger)
    {
        _sourcePath = sourcePath;
        _replicaPath = replicaPath;
        _logger = logger;
    }

    public void Synchronize()
    {
        SyncDirectory(_sourcePath, _replicaPath);
        RemoveExtraFiles(_replicaPath, _sourcePath);
    }
    
    private void RemoveExtraFiles(string replicaPath, string sourcePath)
    {
        foreach (var file in Directory.GetFiles(replicaPath))
        {
            string sourceFile = Path.Combine(sourcePath, Path.GetFileName(file));
            if (!File.Exists(sourceFile))
            {
                try
                {
                    File.Delete(file);
                    _logger.LogInformation($"Deleted extra file: {file}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to delete file: {file}. Error: {ex.Message}");
                }
            }
        }

        foreach (var directory in Directory.GetDirectories(replicaPath))
        {
            string sourceDirectory = Path.Combine(sourcePath, Path.GetFileName(directory));
            if (!Directory.Exists(sourceDirectory))
            {
                try
                {
                    Directory.Delete(directory, true);
                    _logger.LogInformation($"Deleted extra directory: {directory}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to delete directory: {directory}. Error: {ex.Message}");
                }
            }
            else
            {
                RemoveExtraFiles(directory, sourceDirectory);
            }
        }
    }

    private void SyncDirectory(string sourcePath, string replicaPath)
    {
        Directory.CreateDirectory(replicaPath);
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            string destinationFile = Path.Combine(replicaPath, Path.GetFileName(file));
            if (!File.Exists(destinationFile) || File.GetLastWriteTimeUtc(file) > File.GetLastWriteTimeUtc(destinationFile))
            {
                File.Copy(file, destinationFile, true);
                _logger.LogInformation($"Copied file: {file} to {destinationFile}");
            }
        }

        foreach (var directory in Directory.GetDirectories(sourcePath))
        {
            string destinationDirectory = Path.Combine(replicaPath, Path.GetFileName(directory));
            SyncDirectory(directory, destinationDirectory);
        }
    }
}