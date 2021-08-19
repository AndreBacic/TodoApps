using MongoDB.Bson.Serialization.Attributes;

namespace TodoMVCAppAsFastAsICan.Data
{
    [BsonIgnoreExtraElements]
    public class TodoModel
    {
        public string Message { get; set; }
    }
}