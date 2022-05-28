using Jarvis.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService.Contracts;
using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService;

public class WhatIsMyPublicIPService : IIPResolverService
{
    public async Task<string> GetAsync()
    {
        var httpService = new HttpService("https://whatismypublicip.com/");
        var result = await httpService.GetStringAsync(string.Empty);
        var regex = new Regex("<div id=\"up_finished\".*?>(.*?)</div>");
        var ipGroup = regex.Match(result).Groups[1];
        var ip = ipGroup.Value.RemoveCarriageReturnAndOtherFuckingCharacters();
        return ip;
    }
}
