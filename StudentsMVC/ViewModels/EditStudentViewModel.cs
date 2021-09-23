using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.MVC.ViewModels
{
    public class EditStudentViewModel: StudentViewModel
    {
        public IEnumerable<GroupViewModel> Groups { get; set; }
    }
}
