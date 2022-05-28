namespace Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Models;

public class MediaStorageStateModel
{
    public IEnumerable<(string DisplayName, long Size)> Folders { get; private set; }

    public long UsedSpace { get; private set; }

    public long FreeSpace { get; private set; }

    public long TotalSpace { get; private set; }

    public MediaStorageStateModel(
        IEnumerable<(string DisplayName, long Size)> folders,
        long usedSpace,
        long freeSpace,
        long totalSpace)
    {
        Folders = folders;
        UsedSpace = usedSpace;
        FreeSpace = freeSpace;
        TotalSpace = totalSpace;
    }
}
