using System;
using System.Threading;
using System.Threading.Tasks;
using RedLockNet;
using RedLockNet.SERedis;

namespace MiniJobScheduler
{
    public class LockReference : IDisposable, ILockReference
    {
        private readonly RedLockFactory _redLockFactory;
        private readonly CancellationToken _cancellationToken;
        private IRedLock _redLock;

        public LockReference(RedLockFactory redLockFactory,
            CancellationToken cancellationToken)
        {
            _redLockFactory = redLockFactory;
            _cancellationToken = cancellationToken;
        }
        
        public async Task<bool> ClaimAsync(string cluster)
        {
            if (_redLock == null || !_redLock.IsAcquired)
            {
                _redLock = await _redLockFactory.CreateLockAsync(cluster, 
                    TimeSpan.FromSeconds(10), 
                    TimeSpan.FromSeconds(1), 
                    TimeSpan.FromSeconds(1), 
                    _cancellationToken);
            }
            
            return _redLock.IsAcquired;
        }

        public void Dispose()
        {
            _redLock?.Dispose();
        }
    }
}