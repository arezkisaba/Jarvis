namespace Jarvis.Features.Agents.IPResolverAgent.Services.IPResolverService.Contracts;

public interface IIPResolverService
{
    Task<string> GetAsync();
}
