namespace MinecraftMonitoring.Models
{
    public class ContainerStats
    {
        public double CpuPercent { get; set; }

        public long MemoryUsageBytes { get; set; }

        public long MemoryLimitBytes { get; set; }

        public double MemoryPercent { get; set; }
    }
}
