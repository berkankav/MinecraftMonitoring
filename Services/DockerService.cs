using Docker.DotNet;
using Docker.DotNet.Models;
using MinecraftMonitoring.Models;

namespace MinecraftMonitoring.Services
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient _dockerClient;

        public DockerService(IConfiguration configuration)
        {
            var endpoint = configuration["Docker:WindowsTestEndpoint"]
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
                container.Names.Any(name => name == "/minecraft-test"));

            return minecraftContainer != null &&
                   minecraftContainer.State == "running";
        }

        public async Task<ContainerStats?> GetMinecraftStatsAsync()
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true
                });

            var minecraftContainer = containers.FirstOrDefault(container =>
                container.Names.Any(name => name == "/minecraft-test"));

            if (minecraftContainer == null ||
                minecraftContainer.State != "running")
            {
                return null;
            }

            Docker.DotNet.Models.ContainerStatsResponse? stats = null;

            await _dockerClient.Containers.GetContainerStatsAsync(
                minecraftContainer.ID,
                new ContainerStatsParameters
                {
                    Stream = false
                },
                new Progress<Docker.DotNet.Models.ContainerStatsResponse>(
                    response =>
                    {
                        stats = response;
                    }),
                CancellationToken.None);


            if (stats == null)
            {
                return null;
            }

            var cpuDelta =
                stats.CPUStats.CPUUsage.TotalUsage -
                stats.PreCPUStats.CPUUsage.TotalUsage;

            var systemCpuDelta =
                stats.CPUStats.SystemUsage -
                stats.PreCPUStats.SystemUsage;

            var cpuCount =
                stats.CPUStats.OnlineCPUs > 0
                    ? stats.CPUStats.OnlineCPUs
                    : (uint)(stats.CPUStats.CPUUsage.PercpuUsage?.Count ?? 1);

            double cpuPercent = 0;

            if (systemCpuDelta > 0 && cpuDelta > 0)
            {
                cpuPercent =
                    ((double)cpuDelta / systemCpuDelta)
                    * cpuCount
                    * 100.0;
            }

            var memoryUsage = stats.MemoryStats.Usage;
            var memoryLimit = stats.MemoryStats.Limit;

            double memoryPercent = 0;

            if (memoryLimit > 0)
            {
                memoryPercent =
                    ((double)memoryUsage / memoryLimit) * 100.0;
            }

            return new ContainerStats
            {
                CpuPercent = cpuPercent,

                MemoryUsageBytes =
                    checked((long)memoryUsage),

                MemoryLimitBytes =
                    checked((long)memoryLimit),

                MemoryPercent = memoryPercent
            };
        }

    }
}
