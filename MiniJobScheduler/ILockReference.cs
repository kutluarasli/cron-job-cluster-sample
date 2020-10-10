using System.Threading.Tasks;

namespace MiniJobScheduler
{
    public interface ILockReference
    {
        Task<bool> ClaimAsync(string cluster);
    }
}