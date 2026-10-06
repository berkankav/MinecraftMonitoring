namespace MinecraftMonitoring.Services
{
    public interface IDockerService
    {
        Task<bool> IsMinecraftContainerRunningAsync();
    }
}
