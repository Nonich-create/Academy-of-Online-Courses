using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Students.DAL.Enum;

namespace Students.DAL.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string NumberGroup { get; set; }
        public DateTime DateStart { get; set; }
        public EnumGroupStatus GroupStatus { get; set; }
        public int CountMax { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int ManagerId { get; set; }
        public Manager Manager { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public List<Student> Students { get; set; }
     }   
}
