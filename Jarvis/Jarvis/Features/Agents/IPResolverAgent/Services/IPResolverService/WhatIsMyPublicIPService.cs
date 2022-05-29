using Jarvis.Features.Agents.IPResolverAgent.Services.IPResolverService.Contracts;
using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Agents.IPResolverAgent.Services.IPResolverService;

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
