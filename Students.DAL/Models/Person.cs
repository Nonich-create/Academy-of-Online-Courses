using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string URLImagePhoto { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
