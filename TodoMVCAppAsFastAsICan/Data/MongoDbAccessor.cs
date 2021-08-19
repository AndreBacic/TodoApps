using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoMVCAppAsFastAsICan.Data
{
    public class MongoDbAccessor
    {
        private IMongoDatabase db;

        public const string TodoDb = "TodoApp";
        /// <summary>
        /// Stores users and their todos
        /// </summary>
        public const string UserCollection = "Users";

        public const string ModelIdName = "Id";

        /// <summary>
        /// Initialize database using the configuration supplied by DependencyInjection.
        /// </summary>
        /// <param name="configuration"></param>
        public MongoDbAccessor(IConfiguration configuration)
        {
            var client = new MongoClient();
            string database = configuration.GetConnectionString("MongoDB");
            db = client.GetDatabase(database);
        }

        /// <summary>
        /// Initialize database using a known connection string.
        /// </summary>
        /// <param name="database"></param>
        public MongoDbAccessor(string database = TodoDb)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }


        //####################################### BASIC MONGOBD METHODS ##########################################

        public void InsertRecord<T>(T record, string table = UserCollection)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(string table = UserCollection)
        {
            var collection = db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(Guid id, string table = UserCollection)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(ModelIdName, id); // Eq id for equals, ctrl+J to see other comparisons

            return collection.Find(filter).First();
        }

        public void UpsertRecord<T>(Guid id, T record, string table = UserCollection)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", new BsonBinaryData(id, GuidRepresentation.Standard)), // the BsonBinaryData with GuidRep is the not obsolete way
                record,
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(Guid id, string table = UserCollection)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(ModelIdName, id); // Eq id for equals, ctrl+J to see other comparisons
            collection.DeleteOne(filter);
        }
    }
}
