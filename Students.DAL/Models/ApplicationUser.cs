using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.DAL.Models
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        public List<Teacher> Teacher { get; set; }
        public List<Student> Student { get; set; }
        public List<Manager> Manager { get; set; }
    }  
}
