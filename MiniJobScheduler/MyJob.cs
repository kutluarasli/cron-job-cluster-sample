using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MiniJobScheduler
{
    public class MyJob
    {
        private readonly ILockReference _lockReference;
        private readonly IConfiguration _configuration;

        public MyJob(ILockReference lockReference,
            IConfiguration configuration)
        {
            _lockReference = lockReference;
            _configuration = configuration;
        }

        public async void RunAsync(CancellationToken cancellationToken)
        {
            var cluster = _configuration.GetValue<string>("Cluster");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (await _lockReference.ClaimAsync(cluster))
                {
                    Console.WriteLine($"Job has executed at {DateTime.Now}");    
                }
                else
                {
                    Console.WriteLine($"Waiting to acquire lock {DateTime.Now}");
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}