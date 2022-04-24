using Lib.ApiServices.Plex;
using Lib.ApiServices.Tmdb;
using Lib.Core;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Jarvis;

public class MonIPService : IIPResolverService
{
    public async Task<string> GetAsync()
    {
        try
        {
            var httpService = new HttpService("https://mon-ip.io/");
            var result = await httpService.GetStringAsync(string.Empty);
            var regex = new Regex("<p id=\"ip\">(.*?)</p>");
            var ip = regex.Match(result).Groups[1];
            return ip.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
