using System;
using System.ComponentModel.DataAnnotations;

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
