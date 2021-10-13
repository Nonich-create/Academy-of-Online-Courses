using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
    public class DetaliStudentViewModel : StudentViewModel
    {
        public IEnumerable<CourseApplicationViewModel> CourseApplications { get; set; }

    }
}
