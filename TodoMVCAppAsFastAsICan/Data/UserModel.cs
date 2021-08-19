using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoMVCAppAsFastAsICan.Data
{
    [BsonIgnoreExtraElements]
    public class UserModel
    {
        [BsonId]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        [BsonElement("EncryptedPassword")]
        public string PasswordHash { get; set; }
        public List<TodoModel> Todos { get; set; }
    }
}
