using System.Linq;
using System.Net;
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
        public async Task<IActionResult> Get()
        {
            var services = await _consulClient.Agent.Services(HttpContext.RequestAborted);
            var healthServices = await _consulClient.Health.Service("dotnet-service", 
                null, 
                true);

            if (services.StatusCode != HttpStatusCode.OK)
                return BadRequest();
            
            // Should check dependencies
            var pythonServices = services.Response
                .Where(p => p.Value.Service == "dotnet-service")
                .Select(x => x.Value);
            
            return Ok(healthServices.Response);
        }
    }
}