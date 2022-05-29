namespace Jarvis.Features.Services.MediaSizingService.Contracts;

public interface IMediaSizingService
{
    string ConvertBytesToStringWithUnit(
        long bytesCount);
}
