using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Flurl;
using Microsoft.Extensions.Hosting;

namespace DotnetService.Api.HostedServices
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly string _id;
        private readonly Uri _apiUrl;

        private static string ApiUrl => Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "";

        public ConsulHostedService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
            _id = Md5(ApiUrl);
            _apiUrl = new Uri(ApiUrl, UriKind.Absolute);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var agentServiceCheck = new AgentServiceCheck
            {
                HTTP = Url.Combine(_apiUrl.ToString(), "/health"),
                Interval = TimeSpan.FromSeconds(1),
                Timeout = TimeSpan.FromSeconds(5)
            };
            
            var agentServiceRegistration = new AgentServiceRegistration
            {
                Address = _apiUrl.Host,
                ID = _id,
                Name = "dotnet-service",
                Port = _apiUrl.Port,
                Check = agentServiceCheck
            };
            
            //Should handle this result
            try
            {
                _ = await _consulClient.Agent.ServiceRegister(agentServiceRegistration, cancellationToken);
            }
            catch
            {
                // ignored
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //Should handle this result
            try
            {
                _ = await _consulClient.Agent.ServiceDeregister(_id, cancellationToken);
            }
            catch
            {
                // ignored
            }
        }

        private static string Md5(string s)
        {
            using var provider = System.Security.Cryptography.MD5.Create();
            var builder = new StringBuilder();                           

            foreach (var b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
}