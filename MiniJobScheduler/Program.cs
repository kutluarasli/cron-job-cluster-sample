using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace MiniJobScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            
            var redLockFactory = CreateRedLockFactory();
            var lockReference = TryAcquireLock(redLockFactory, cancellationTokenSource.Token);

            //Start job
            var myJob = new MyJob(lockReference);
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
            var redisEndpoint = new RedLockEndPoint(new DnsEndPoint("localhost", 6379))
            {
                RedisDatabase = 0,
                RedisKeyFormat = "MiniJobScheduler:{0}",
            };
            var listOfRedisEndpoints = new List<RedLockEndPoint> {redisEndpoint};
            var redlockFactory = RedLockFactory.Create(listOfRedisEndpoints);
            return redlockFactory;
        }
    }
}