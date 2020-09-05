using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiniJobScheduler
{
    public class MyJob
    {
        private readonly ILockReference _lockReference;

        public MyJob(ILockReference lockReference)
        {
            _lockReference = lockReference;
        }

        public async void RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (await _lockReference.ClaimAsync())
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