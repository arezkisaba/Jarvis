using Jarvis.Features.Agents.MediaStorageAgent.Services.MediaStorageService.Contracts;

namespace Jarvis.Features.Agents.MediaStorageAgent.Services.MediaStorageService;

public class MediaStorageService : IMediaStorageService
{
    public IEnumerable<(string DisplayName, long Size)> GetFolderContentAndSize(
        string folderPath)
    {
        var result = new List<(string displayName, long size)>();
        var folders = Directory.GetDirectories(folderPath);
        foreach (var folder in folders)
        {
            var folderSize = GetFolderSize(folder);
            var folderName = new DirectoryInfo(folder).Name;
            if (folderName == "$RECYCLE.BIN" || folderName == "System Volume Information")
            {
                continue;
            }

            result.Add((folderName, folderSize));
        }

        return result;
    }

    public (long UsedSpace, long FreeSpace, long TotalSpace) GetDriveInformations(
        string folderPath)
    {
        var drive = Path.GetPathRoot(new FileInfo(folderPath).FullName);
        var driveInfo = new DriveInfo(drive);
        if (!driveInfo.IsReady)
        {
            throw new InvalidOperationException($"Drive {drive} not ready");
        }

        return (driveInfo.TotalSize - driveInfo.AvailableFreeSpace, driveInfo.AvailableFreeSpace, driveInfo.TotalSize);
    }

    #region Private use

    private long GetFolderSize(
        string folderPath)
    {
        var folderSize = 0L;

        try
        {
            if (!Directory.Exists(folderPath))
            {
                return folderSize;
            }
            else
            {
                try
                {
                    foreach (var file in Directory.GetFiles(folderPath))
                    {
                        if (File.Exists(file))
                        {
                            var finfo = new FileInfo(file);
                            folderSize += finfo.Length;
                        }
                    }

                    foreach (var dir in Directory.GetDirectories(folderPath))
                    {
                        folderSize += GetFolderSize(dir);
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
        }

        return folderSize;
    }

    #endregion
}
