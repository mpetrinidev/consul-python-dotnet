using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;

namespace DotnetService.Api.HostedServices
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly string _id;
        private readonly Uri _apiUrl;

        private static string ApiUrl => Environment.GetEnvironmentVariable("API_URL") ?? "";

        public ConsulHostedService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
            _id = Md5(ApiUrl);
            _apiUrl = new Uri(ApiUrl, UriKind.Absolute);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var agentServiceRegistration = new AgentServiceRegistration
            {
                Address = _apiUrl.Host,
                ID = _id,
                Name = "dotnet-service",
                Port = _apiUrl.Port
            };
            
            //Should handle this result
            _ = await _consulClient.Agent.ServiceRegister(agentServiceRegistration, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _ = await _consulClient.Agent.ServiceDeregister(_id, cancellationToken);
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