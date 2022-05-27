using System.Text.RegularExpressions;

namespace Jarvis;

public interface IMediaMatchingService
{
    (MediaTypeModel? mediaType, Match match) GetMediaTypeAndInformations(
           string content);
}
