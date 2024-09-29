using ParkingManagementSystem.BL.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _cache = redisConnection.GetDatabase();
        }

        public async Task RemoveValueAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task ClearCache()
        {

            var redisEndpoints = _redisConnection.GetEndPoints(true);
            foreach (var redisEndpoint in redisEndpoints)
            {
                var redisServer = _redisConnection.GetServer(redisEndpoint);
                redisServer.FlushAllDatabases();
            }
        }

        public async Task<IDictionary<string, string>> GetAllAsync()
        {
            var allKeysValues = new Dictionary<string, string>();
            var redisEndpoints = _redisConnection.GetEndPoints(true);

            foreach (var redisEndpoint in redisEndpoints)
            {
                var redisServer = _redisConnection.GetServer(redisEndpoint);
                var keys = redisServer.Keys();

                foreach (var key in keys)
                {
                    string value = await _cache.StringGetAsync(key);
                    allKeysValues[key.ToString()] = value;
                }
            }

            return allKeysValues;
        }

        public async Task<string> GetValueAsync(string key)
        {
            return await _cache.StringGetAsync(key);
        }

        public async Task<bool> SetValueAsync(string key, string value)
        {
            return await _cache.StringSetAsync(key, value, TimeSpan.FromHours(1));
        }
    }
}
