using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TodoMVCAppAsFastAsICan.Data
{
    [BsonIgnoreExtraElements]
    public class UserModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }
        public string EmailAddress { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }
        public List<TodoModel> Todos { get; set; } = new List<TodoModel>();
    }
}
