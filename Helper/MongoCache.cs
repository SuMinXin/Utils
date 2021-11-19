using System;
using System.Collections.Generic;
using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Utils.Setting;

namespace Utils {
    public class CacheHelper : IMongoCache {
        private MongoHelper mongodb { get; set; }
        private MongoCollection collection { get; set; }
        public CacheHelper(MongoHelper mongodb, MongoCollection collection) {
            this.mongodb = mongodb;
            this.collection = collection;
            mongodb.AddTTL(collection);
        }

        public void insertCache(string key, object value, int expireMin) {
            try {
                CachePara para = new CachePara(expireMin) {
                    key = key,
                    createTime = DateTime.Now,
                    data = GZip.compress(JsonSerializer.Serialize(value))
                };
                mongodb.Insert(collection, para);
            } catch {
                //Allow to fail
            }
        }

        public T getCache<T>(string key, int extendMin = 0) {
            FilterDefinition<CachePara> filter = Builders<CachePara>.Filter.Where(e => e.key.Equals(key));
            // 取最新一筆
            SortDefinition<CachePara> sort = new SortDefinitionBuilder<CachePara>().Descending("createTime");
            List<CachePara> paras = mongodb.Find(collection, filter, string.Empty, sort);
            if (Compare.EmptyCollection(paras)) {
                return default(T);
            }
            if (extendMin > 0) {
                extendCache(key, extendMin);
            }
            return JsonSerializer.Deserialize<T>(GZip.decompress(paras[0].data));
        }

        public void clearCache(string key) {
            FilterDefinition<CachePara> filter = Builders<CachePara>.Filter.Where(e => e.key.Equals(key));
            mongodb.DeleteMany(collection, filter);
        }

        public void extendCache(string key, int extendMin) {
            FilterDefinition<CachePara> filter = Builders<CachePara>.Filter.Where(e => e.key.Equals(key));
            UpdateDefinition<CachePara> update = Builders<CachePara>.Update.Set("expireAt", DateTime.Now.AddMinutes(extendMin));
            mongodb.UpdateOne(collection, filter, update);
        }

        [BsonIgnoreExtraElements]
        public class CachePara {
            public CachePara(int minutes = 30) {
                expireAt = DateTime.Now.AddMinutes(minutes);
            }
            public string key { get; set; }
            public string data { get; set; }
            public DateTime createTime { get; set; }

            [BsonElement("expireAt")]
            public DateTime expireAt { get; set; }
        }
    }
}