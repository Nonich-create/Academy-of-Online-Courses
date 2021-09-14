using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Manager : Person
    {
        public List<Group> Groups { get; set; }
    }
}
