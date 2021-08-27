using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TodoMVCAppAsFastAsICan.Data
{
    [BsonIgnoreExtraElements]
    public class TodoModel
    {
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastEdited { get; set; }
    }
}