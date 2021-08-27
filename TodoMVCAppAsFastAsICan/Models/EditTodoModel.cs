using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoMVCAppAsFastAsICan.Models
{
    public class EditTodoModel
    {
        public int Index { get; set; }
        [Required]
        public string Message { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edited")]
        public DateTime LastEdited { get; set; }
    }
}
