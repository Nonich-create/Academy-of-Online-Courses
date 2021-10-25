using Students.DAL.Models.Base;
using System.Collections.Generic;
using System;

namespace Students.DAL.Models
{
    public class Course : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Visible { get; set; }
        public string URLImagePhoto { get; set; }
        public uint Duration { get; set; } 
    }
}
 
