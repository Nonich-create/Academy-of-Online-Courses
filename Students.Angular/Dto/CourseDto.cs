using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.Angular.Dto
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Duration { get; set; }
        public string URLImagePhoto { get; set; }
        public bool Visible { get; set; } = false;
    }
}
