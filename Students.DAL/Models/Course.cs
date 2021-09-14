using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Visible { get; set; }
        public string URLImagePhoto { get; set; }
        public string Duration { get; set; }
        //public List<Lesson> Lessons { get; set; }
        //public List<Group> Groups { get; set; }
    }
}
 
