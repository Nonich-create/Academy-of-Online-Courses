using Students.DAL.Models.Base;

namespace Students.DAL.Models
{
    public class Person : BaseModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string URLImagePhoto { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
