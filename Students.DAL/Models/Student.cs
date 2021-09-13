using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models
{
    public class Student : Person
    {
        [Key]
        public int StudentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public List<Assessment> Assessments { get; set; }
        public List<ApplicationCourse> ApplicationCourse { get; set; }
    }
}
