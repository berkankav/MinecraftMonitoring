using Microsoft.AspNetCore.Mvc;
using MinecraftMonitoring.Models;
using MinecraftMonitoring.Services;

namespace MinecraftMonitoring.Controllers
{
    [ApiController]
    [Route ("api/server")]
    public class ServerController : Controller
    {
        private readonly IDockerService _dockerService;

        public ServerController(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        [HttpGet ("status")]
        public async Task<ActionResult<ServerStatus>> GetStatus ()
        {
            var online = await _dockerService.IsMinecraftContainerRunningAsync();

            return Ok(new ServerStatus
            {
                Online = online,
                ContainerName = "minecraft"
            });
        }
    }
}
