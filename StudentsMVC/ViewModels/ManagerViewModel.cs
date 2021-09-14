using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.MVC.ViewModels
{
 
    public class ManagerViewModel: PersonViewModel
    {
     
        public List<GroupViewModel> Groups { get; set; }
    }
}
