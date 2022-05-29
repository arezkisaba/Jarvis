using Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.Services.IPResolverService.Contracts;
using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.Services.IPResolverService;

public class MonIPService : IIPResolverService
{
    public async Task<string> GetAsync()
    {
        var httpService = new HttpService("https://mon-ip.io/");
        var result = await httpService.GetStringAsync(string.Empty);
        var regex = new Regex("<p id=\"ip\">(.*?)</p>");
        var ipGroup = regex.Match(result).Groups[1];
        var ip = ipGroup.Value.RemoveCarriageReturnAndOtherFuckingCharacters();
        return ip;
    }
}
