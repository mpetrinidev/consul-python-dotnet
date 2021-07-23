using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotnetService.Api.Controllers
{
    [ApiController, Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public Task Get()
        {
            // Should check dependencies
            return Task.CompletedTask;
        }
    }
}