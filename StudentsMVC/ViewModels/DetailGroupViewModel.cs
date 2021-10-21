using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
    public class DetailGroupViewModel : GroupViewModel
    {
        public IEnumerable<StudentViewModel> Students { get; set; }
    }
}
