using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
    public class DetalisCourseViewModel : CourseViewModel
    {
        public IEnumerable<CourseApplicationViewModel> CourseApplications { get; set; } 

    }
}
