using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL_SIMPLE
{
    public static class RedisDataAccess
    {
        public static bool SaveWithTimeStamp(string key, string value)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient("localhost"))
            {

                isSuccess = redisClient.Set(key,
                    JsonConvert.SerializeObject(new RedisObject()
                    { JsonData = value, LastUpdateTime = DateTime.Now }) );
            }
            return isSuccess;
        }

        public static IRedisObject GetWithTimeStamp(string key)
        {
            using (RedisClient redisClient = new RedisClient("localhost"))
            {
                string result = redisClient.Get<string>(key);
                if (result != null)
                    return JsonConvert.DeserializeObject<RedisObject>(result);
                return null;
            }
        }
    }
}
