using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;

namespace DotnetService.Api.Controllers
{
    [ApiController, Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConsulClient _consulClient;

        public HomeController(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }
        
        [HttpGet]
        public async Task Get()
        {
            var services = await _consulClient.Agent.Services(HttpContext.RequestAborted);

            // Should check dependencies
            
        }
    }
}