using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Configuration;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace MiniJobScheduler
{
    static class Program
    {
        public static IConfiguration Configuration { get; private set; }
        
        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            
            var redLockFactory = CreateRedLockFactory();
            var lockReference = TryAcquireLock(redLockFactory, cancellationTokenSource.Token);

            //Start job
            var myJob = new MyJob(lockReference, Configuration);
            myJob.RunAsync(cancellationTokenSource.Token);

            //Wait for termination
            Console.ReadKey();
            
            //Finalize process
            lockReference.Dispose();
            redLockFactory.Dispose();
            cancellationTokenSource.Cancel();
        }

        private static LockReference TryAcquireLock(RedLockFactory redLockFactory, CancellationToken token)
        {
            var redLockReference = new LockReference(redLockFactory, token);
            
            return redLockReference;
        }

        private static RedLockFactory CreateRedLockFactory()
        {
            var host = Configuration.GetValue<string>("Host");
            var port = Configuration.GetValue<int>("Port");
            
            var redisEndpoint = new RedLockEndPoint(new DnsEndPoint(host, port))
            {
                RedisDatabase = 0,
                RedisKeyFormat = "MiniJobScheduler:{0}",
            };
            var listOfRedisEndpoints = new List<RedLockEndPoint> {redisEndpoint};
            var redLockFactory = RedLockFactory.Create(listOfRedisEndpoints);
            return redLockFactory;
        }
    }
}