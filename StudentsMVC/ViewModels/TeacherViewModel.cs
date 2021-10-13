using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
    public class TeacherViewModel: PersonViewModel
    {
        public IEnumerable<GroupViewModel> Groups { get; set; }

    }
}
