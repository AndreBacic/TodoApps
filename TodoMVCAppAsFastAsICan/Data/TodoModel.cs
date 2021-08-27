using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace TodoMVCAppAsFastAsICan.Data
{
    [BsonIgnoreExtraElements]
    public class TodoModel
    {
        // todo: make TodoViewModel IF the mvc annotations conflict with the mongodb ones.
        [Required]
        public string Message { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime LastEdited { get; set; }
    }
}