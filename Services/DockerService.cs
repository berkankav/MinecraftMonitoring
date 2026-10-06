using Docker.DotNet;
using Docker.DotNet.Models;

namespace MinecraftMonitoring.Services
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient _dockerClient;

        public DockerService(IConfiguration configuration)
        {
            var endpoint = configuration["Docker:Endpoint"]
                ?? throw new InvalidOperationException(
                    "Docker endpoint is not configured, please add it in the appsettings.json");

            _dockerClient = new DockerClientConfiguration(
            new Uri(endpoint))
            .CreateClient();

        }
        public async Task<bool> IsMinecraftContainerRunningAsync()
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true
            });

            var minecraftContainer = containers.FirstOrDefault(container =>
            container.Names.Any(name => name == "/minecraft"));

            return minecraftContainer != null && minecraftContainer.State == "running";

        }
    }
}
