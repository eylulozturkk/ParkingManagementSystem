using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Interface
{
    public interface IRedisCacheService : IBusinessUnit
    {
        Task<string> GetValueAsync(string key);
        Task<bool> SetValueAsync(string key, string value);
        Task RemoveValueAsync(string key);
        Task ClearCache();

    }
}
