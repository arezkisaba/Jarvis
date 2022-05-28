using Jarvis.Services.MediaMatchingService.Models;
using System.Text.RegularExpressions;

namespace Jarvis.Services.MediaMatchingService.Contracts;

public interface IMediaMatchingService
{
    (MediaTypeModel? mediaType, Match match) GetMediaTypeAndInformations(
           string content);
}
