using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL_SIMPLE
{
    public interface IRedisObject
    {
        DateTime LastUpdateTime { get; set; }
        string JsonData { get; set; }
    }
    public class RedisObject : IRedisObject
    {
        public DateTime LastUpdateTime { get; set; }
        public string JsonData { get; set; }
    }
}
