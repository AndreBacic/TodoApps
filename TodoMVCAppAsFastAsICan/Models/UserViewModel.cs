using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoMVCAppAsFastAsICan.Data;

namespace TodoMVCAppAsFastAsICan.Models
{
    public class UserViewModel
    {
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
        public string PhoneNumber { get; set; }

        public List<TodoModel> Todos { get; set; }
    }
}
