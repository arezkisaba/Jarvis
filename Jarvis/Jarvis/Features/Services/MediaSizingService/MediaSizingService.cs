using Jarvis.Features.Services.MediaSizingService.Contracts;

namespace Jarvis.Features.Services.MediaSizingService;

public class MediaSizingService : IMediaSizingService
{
    public string ConvertBytesToStringWithUnit(
        long bytesCount)
    {
        var unit = "B";
        var coef = 1024d;
        var newSize = Convert.ToDouble(bytesCount);

        if (newSize / coef >= 1)
        {
            newSize /= coef;
            unit = "KB";
        }

        if (newSize / coef >= 1)
        {
            newSize /= coef;
            unit = "MB";
        }

        if (newSize / coef >= 1)
        {
            newSize /= coef;
            unit = "GB";
        }

        if (newSize / coef >= 1)
        {
            newSize /= coef;
            unit = "TB";
        }

        return $"{newSize.ToString("0.##")} {unit}";
    }
}
