using Students.DAL.Models;
using Students.DAL.Specifications.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.DAL.Specifications
{
    public class StudentSearchWithItemsSpecifications : BaseSpecification<Student>
    {
        public StudentSearchWithItemsSpecifications(string userId)
      : base(s => s.UserId == userId)
        {
            AddInclude(s => s.User);
            AddInclude(s => s.Group);
            AddInclude(s => s.Group.Course);
        }
    }
}
