namespace Jarvis;

public interface IMediaStorageService
{
    IEnumerable<(string DisplayName, long Size)> GetFolderContentAndSize(
        string folderPath);

    (long UsedSpace, long FreeSpace, long TotalSpace) GetDriveInformations(
        string folderPath);
}
