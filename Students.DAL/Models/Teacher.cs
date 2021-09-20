using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Teacher : Person
    {
 
        public List<Group> Groups { get; set; }
    }
}
