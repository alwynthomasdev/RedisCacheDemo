using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheDemo
{
    public interface IRedisObjectStore<T>
    {
        /// <summary>
        /// Get an object stored in redis by key
        /// </summary>
        /// <param name="key">The key used to stroe object</param>
        T Get(string key);
        
        /// <summary>
        /// Save an object in redis
        /// </summary>
        /// <param name="key">The key to stroe object against</param>
        /// <param name="obj">The object to store</param>
        void Save(string key, T obj);

        /// <summary>
        /// Delete an object from redis using a key
        /// </summary>
        /// <param name="key">The key the object is stored using</param>
        void Delete(string key);
    }

    public class RedisObjectStore<T> : IRedisObjectStore<T>
    {
        private readonly IDatabase _DB;

        public RedisObjectStore(IDatabase DB)
        {
            _DB = DB;
        }

        public T Get(string key)
        {
            key = GenerateKey(key);
            var hash = _DB.HashGetAll(key);

            return MapFromHash(hash);
        }

        public void Save(string key, T obj)
        {
            if(obj!=null)
            {
                var hash = GenerateRedisHash(obj);
                   key = GenerateKey(key);

                _DB.HashSet(key, hash);
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Contains(":"))
                throw new ArgumentException("invalid key");

            key = GenerateKey(key);
            _DB.KeyDelete(key);
        }

        #region Helpers

        //generate a key from a given key and the class name of the object we are storing
        string GenerateKey(string key) =>
            string.Concat(key.ToLower(), ":", NameOfT.ToLower());

        //create a hash entry array from object using reflection
        HashEntry[] GenerateRedisHash(T obj)
        {
            var props = PropertiesOfT;
            var hash = new HashEntry[props.Count()];

            for (int i = 0; i < props.Count(); i++)
                hash[i] = new HashEntry(props[i].Name, props[i].GetValue(obj).ToString());

            return hash;
        }

        //build object from hash entry array using reflection
        T MapFromHash(HashEntry[] hash)
        {
            var obj = (T)Activator.CreateInstance(TypeOfT);//new instance of T
            var props = PropertiesOfT;

            for (int i = 0; i < props.Count(); i++)
                for (int j = 0; j < hash.Count(); j++)
                    if (props[i].Name == hash[j].Name)
                    {
                        var val = hash[j].Value;
                        var type = props[i].PropertyType;

                        if(type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            if(string.IsNullOrEmpty(val))
                                props[i].SetValue(obj, null);

                        props[i].SetValue(obj, Convert.ChangeType(val, type));
                    }

            return obj;
        }

        Type TypeOfT { get { return typeof(T); } }

        string NameOfT { get { return TypeOfT.FullName; } }

        PropertyInfo[] PropertiesOfT { get { return TypeOfT.GetProperties(); } }

        #endregion

    }
}
