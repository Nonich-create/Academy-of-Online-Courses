using Students.DAL.Models.Base;
using System.Collections.Generic;

namespace Students.DAL.Models
{
    public class Course : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Visible { get; set; }
        public string URLImagePhoto { get; set; }
        public string Duration { get; set; } // менять на дату
        public List<Lesson> Lessons { get; set; }
        public List<Group> Groups { get; set; }
    }
}
 
