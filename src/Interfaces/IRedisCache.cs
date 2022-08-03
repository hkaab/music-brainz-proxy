using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRedisCache
    {
        Task<T> TryOperation<T>(Func<IDatabase, Task<T>> op);

        Task TryOperation(Func<IDatabase, Task> op);
    }
}
