using MongoDB.Driver;
using System.Configuration;
using StackExchange.Redis;

namespace Rdio.Models
{
    public class NoSql
    {

        //Redis DB
        private static ConnectionMultiplexer redis = null;

        public static ConnectionMultiplexer Redis
        {
            get
            {
                if (redis == null)
                     redis = ConnectionMultiplexer.Connect("localhost");
                return redis;
            }
        }



        //Mongo DB
        private static IMongoDatabase instance;
        private static IMongoDatabase audit;
        private NoSql() { }
        public static IMongoDatabase Instance
        {
            get
            {
                //return new MongoClient().GetDatabase(ConfigurationManager.AppSettings["mongodbname"]);
                if (instance == null)
                {
                    instance = new MongoClient().GetDatabase(ConfigurationManager.AppSettings["maindb"]);
                }
                return instance;
            }
        }

        public static IMongoDatabase Audit
        {
            get
            {
                if (audit == null)
                {
                    audit = new MongoClient().GetDatabase(ConfigurationManager.AppSettings["auditdb"]);
                }
                return audit;
            }
        }

    }
}