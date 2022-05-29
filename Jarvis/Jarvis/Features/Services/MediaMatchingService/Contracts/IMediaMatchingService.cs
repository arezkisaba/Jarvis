using Jarvis.Features.Services.MediaMatchingService.Models;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Services.MediaMatchingService.Contracts;

public interface IMediaMatchingService
{
    (MediaTypeModel? mediaType, Match match) GetMediaTypeAndInformations(
           string content);
}
