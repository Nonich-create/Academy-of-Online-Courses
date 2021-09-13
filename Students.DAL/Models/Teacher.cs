using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Teacher : Person
    {
        [Key]
        public int TeacherId { get; set; }
        public List<Group> Groups { get; set; }
    }
}
