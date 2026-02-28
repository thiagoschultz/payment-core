using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Cache
{
    public class RedisService
    {

        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, object value)
        {
            var json = JsonSerializer.Serialize(value);

            await _db.StringSetAsync(key, json);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }

    }
}