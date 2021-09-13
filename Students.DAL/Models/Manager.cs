using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Manager : Person
    {
        [Key]
        public int ManagerId { get; set; }
        public List<Group> Groups { get; set; }
    }
}
