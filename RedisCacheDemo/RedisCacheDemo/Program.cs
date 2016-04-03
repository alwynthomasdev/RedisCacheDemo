using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheDemo
{

    //TODO: write demo

    class Program
    {
        //static int _iterations = 10000;

        static void Main(string[] args)
        {

            var RedisConnection = RedisConnectionFactory.GetConnection();

            var db = RedisConnection.GetDatabase();

            //for (int i = 0; i < _iterations; i++)
            //{
            //    var key = string.Format("k_{0}", i);
            //    var value = RandomVal();
            //    db.StringSet(key, value);
            //}

            //Console.WriteLine("Data Saved...");
            //Console.ReadLine();
            //Console.WriteLine("\n");

            //for (int i = 0; i < _iterations; i++)
            //{
            //    var key = string.Format("k_{0}", i);
            //    var value = (int)db.StringGet(key);
            //    Console.WriteLine("{0} => {1}", key, value);
            //}

            //var randomDataSore = new RedisObjectStore<RandomData>(db);
            //var dataIn = new RandomData { ID = 1, StringData = "Some String Data!", IntegerData = 1234 };
            //randomDataSore.Save(dataIn.ID.ToString(), dataIn);

            //var dataOut = randomDataSore.Get("1");

            //randomDataSore.Delete("1");

            //dataOut = randomDataSore.Get("1");

            //Console.WriteLine("\n");
            //Console.ReadLine();

        }

        static int RandomVal() =>
            new Random(DateTime.Now.Millisecond).Next(1111, 9999);

    }

    public class RandomData
    {
        public int ID { get; set; }
        public string StringData { get; set; }
        public int IntegerData { get; set; }
    }
}
