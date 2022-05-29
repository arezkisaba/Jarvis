using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Models;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Services.MediaMatchingService;

public class MediaMatchingService : IMediaMatchingService
{
    public readonly IList<string> EpisodeExpressions = new List<string>
    {
        @"^(.*?)[ \.]?S[ ]?([0-9]{1,})[ ]?E[ ]?([0-9]{1,})(.*?)$", // Visitors S01E07
        @"^(.*?)[ \.]?Saison[ ]?([0-9]{1,})[ ]?Episode[ ]?([0-9]{1,})(.*?)$", // Visitors Saison 01 Episode 07

    }.AsReadOnly();

    public readonly IList<string> SeasonExpressions = new List<string>
    {
        @"^(.*?)[ ]?S[ ]?([0-9]{1,})(.*?)$", // Visitors S01
        @"^(.*?)[ ]?Saison[ ]?([0-9]{1,})(.*?)$", // Visitors Saison 01

    }.AsReadOnly();

    public readonly IList<string> MovieExpressions = new List<string>
    {
        @"^(.*?)$", // Avengers

    }.AsReadOnly();

    public (MediaTypeModel? mediaType, Match match) GetMediaTypeAndInformations(
        string content)
    {
        var contentTransformed = content;

        foreach (var expression in EpisodeExpressions)
        {
            var match = new Regex(expression).Match(contentTransformed);
            if (match.Success)
            {
                return (MediaTypeModel.Episode, match);
            }
        }

        foreach (var expression in SeasonExpressions)
        {
            var match = new Regex(expression).Match(contentTransformed);
            if (match.Success)
            {
                return (MediaTypeModel.Season, match);
            }
        }

        foreach (var expression in MovieExpressions)
        {
            var match = new Regex(expression).Match(contentTransformed);
            if (match.Success)
            {
                return (MediaTypeModel.Movie, match);
            }
        }

        return (null, null);
    }

    public string[] GetPossibleMovieTitles(
        string torrentTitle)
    {
        var indexes = new List<int>();
        var titles = new List<string>()
        {
            torrentTitle
        };
        
        for (var i = torrentTitle.Length - 1; i >= 0; i--)
        {
            if (torrentTitle[i] == ' ' || torrentTitle[i] == '.')
            {
                indexes.Add(i);
            }
        }

        foreach (var index in indexes)
        {
            titles.Add(torrentTitle[..index]);
        }

        return titles.ToArray();
    }
}
