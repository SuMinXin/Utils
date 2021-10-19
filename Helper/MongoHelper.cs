using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Connection;
using MongoDB.Bson;
using MongoDB.Driver;
using Utils.Setting;

namespace Utils {
    public class MongoHelper {
        public enum EnumSortType {
            ASC,
            DESC
        }

        public class SortDef {
            public string FieldName { get; set; }
            public EnumSortType SortType { get; set; }
        }

        private IMongoClient _client;
        private IMongoDatabase _database;

        public MongoHelper(string connectionString, string database) {
            _client = new MongoClient(Decrypt.get(connectionString));
            string db = MongoUrl.Create(string.Format(Decrypt.get(connectionString), Decrypt.get(database))).DatabaseName;
            _database = _client.GetDatabase(db);
        }

        #region Methods
        /// <summary> Find </summary>
        public T FindOne<T>(MongoCollection mCollection, FilterDefinition<T> filter, bool monthYear = false, string suffix = "") {
            string collectionName = string.Concat(connectionName(mCollection, monthYear), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            return collection.Find(filter).ToList().FirstOrDefault();
        }
        /// <summary> Find </summary>
        public List<T> Find<T>(MongoCollection mCollection, FilterDefinition<T> pFilter, SortDefinition<T> pSort = null, int? pLimit = null) {
            IMongoCollection<T> collection = _database.GetCollection<T>(connectionName(mCollection));
            return collection.Find(pFilter).Sort(pSort).Limit(pLimit).ToList();
        }

        /// <summary> Find </summary>
        public List<T> Find<T>(MongoCollection mCollection, FilterDefinition<T> pFilter, string suffix, SortDefinition<T> pSort = null, int? pLimit = null) {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            return collection.Find(pFilter).Sort(pSort).Limit(pLimit).ToList();
        }

        /// <summary> FindAll </summary>
        public List<T> FindAll<T>(MongoCollection mCollection, string suffix = "", SortDefinition<T> pSort = null, int? pLimit = null) {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            return collection.Find(new BsonDocument()).Sort(pSort).Limit(pLimit).ToList();
        }

        /// <summary> FindAndModify - Queue</summary>
        public T FindAndModify<T>(MongoCollection mCollection, SortDefinition<T> pSort = null, int? pLimit = null) {
            IMongoCollection<T> collection = _database.GetCollection<T>(connectionName(mCollection));
            return collection.FindOneAndDelete<T>(new BsonDocument());
        }

        /// <summary> FindAndModify </summary>
        public T FindAndModify<T>(MongoCollection pCollection, FilterDefinition<T> filter) {
            string collectionName = pCollection.ToString();
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            return collection.FindOneAndDelete<T>(filter);
        }

        /// <summary> Insert List </summary>
        public void InsertList<T>(MongoCollection mCollection, IEnumerable<T> pDocList, string suffix = "") {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            collection.InsertMany(pDocList);
        }

        /// <summary> Insert Doc </summary>
        public void Insert<T>(MongoCollection mCollection, T pDoc, bool yearMonth = false) {
            IMongoCollection<T> collection = _database.GetCollection<T>(connectionName(mCollection, yearMonth));
            collection.InsertOne(pDoc);
        }

        /// <summary> Insert Doc </summary>
        public void Insert<T>(MongoCollection mCollection, T pDoc, string suffix) {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            collection.InsertOne(pDoc);
        }

        /// <summary> Insert Doc </summary>
        public bool InsertWithResult<T>(MongoCollection mCollection, T pDoc, bool yearMonth = false) {
            Insert(mCollection, pDoc, yearMonth);
            string collectionName = connectionName(mCollection, yearMonth);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName.ToString());
            return collection.Find(pDoc.ToBsonDocument()).FirstOrDefault() != null;
        }
        /// <summary> Update Doc </summary>
        public void UpdateOne<T>(MongoCollection mCollection, FilterDefinition<T> filter, UpdateDefinition<T> update) {
            IMongoCollection<T> collection = _database.GetCollection<T>(connectionName(mCollection));
            collection.UpdateOne(filter, update);
        }

        /// <summary> Update Doc </summary>
        public void UpdateAll<T>(MongoCollection mCollection, FilterDefinition<T> filter, UpdateDefinition<T> update, string suffix = "") {
            string collectionName = string.Concat(connectionName(mCollection), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            collection.UpdateMany(filter, update);
        }
        /// <summary> Replace Document </summary>
        public void ReplaceOne<T>(MongoCollection mCollection, FilterDefinition<T> filter, T data, string suffix = "") {
            string collectionName = string.Concat(connectionName(mCollection), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            collection.ReplaceOne(filter, data);
        }

        /// <summary> Replace Document </summary>
        public T ReplaceWithResult<T>(MongoCollection mCollection, FilterDefinition<T> filter, T data) {
            IMongoCollection<T> collection = _database.GetCollection<T>(mCollection.ToString());
            string res = collection.ReplaceOne(filter, data).ToJson();
            return string.IsNullOrWhiteSpace(res) ? default(T) : JsonSerializer.Deserialize<T>(res);
        }

        /// <summary>Upsert (Insert) </summary>
        public bool UpsertInsert<T>(MongoCollection mCollection, FilterDefinition<T> filter, T data, string suffix = "") {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            var options = new FindOneAndReplaceOptions<T, T>() { ReturnDocument = ReturnDocument.After, IsUpsert = true };
            T newdoc = collection.FindOneAndReplace(filter, data, options);
            return newdoc != null;
        }

        /// <summary> Delete Collection </summary>
        public long DeleteAll<T>(MongoCollection mCollection) {
            IMongoCollection<T> collection = _database.GetCollection<T>(connectionName(mCollection));
            return collection.DeleteMany(new BsonDocument()).DeletedCount;
        }

        public long DeleteMany<T>(MongoCollection mCollection, FilterDefinition<T> filter, string suffix = "") {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            return collection.DeleteMany(filter).DeletedCount;
        }

        public List<BsonDocument> GetUserInfo(MongoCollection mCollection, string token) {
            var collection = _database.GetCollection<BsonDocument>(connectionName(mCollection));
            var Filter = new BsonDocument("ID", token); //設定過濾條件  
            return collection.Find(Filter).ToList();
        }

        public void AddTTL(MongoCollection mCollection, int expireHour = 3, string suffix = "") {
            string collectionName = string.Concat(mCollection.ToString(), suffix);
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };
            if (!_database.ListCollectionNames(options).Any()) {
                var keys = Builders<BsonDocument>.IndexKeys.Ascending("expireAt");
                var indexOptions = new CreateIndexOptions { ExpireAfter = new TimeSpan(0, expireHour, 0) };
                var model = new CreateIndexModel<BsonDocument>(keys, indexOptions);
                _database.GetCollection<BsonDocument>(collectionName).Indexes.CreateOne(model);
            }
        }

        private string connectionName(MongoCollection mCollection, bool yearMonth = false) {
            if (yearMonth) {
                return string.Concat(mCollection.ToString(), DateTimes.yearMonth());
            }
            return mCollection.ToString();
        }
        #endregion
    }
}