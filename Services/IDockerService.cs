using MinecraftMonitoring.Models;

namespace MinecraftMonitoring.Services
{
    public interface IDockerService
    {
        Task<bool> IsMinecraftContainerRunningAsync();
        Task<ContainerStats?> GetMinecraftStatsAsync();
    }
}
