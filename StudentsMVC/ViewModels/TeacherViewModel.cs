using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Students.MVC.ViewModels
{
    public class TeacherViewModel: PersonViewModel
    {
        [Key]
        public int TeacherId { get; set; }
        public List<GroupViewModel> Groups { get; set; }
    }
}
