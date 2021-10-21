using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.DAL.Models
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
    }  
}
