using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
 
    public class ManagerViewModel: PersonViewModel
    {
        public IEnumerable<GroupViewModel> Groups { get; set; }
    }
}
