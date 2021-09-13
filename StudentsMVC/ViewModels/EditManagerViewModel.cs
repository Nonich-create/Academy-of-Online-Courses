using Students.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class EditManagerViewModel
    {
        public int ManagerId { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Не указано отчество")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        [UIHint("Url")]
        [Display(Name = "URL")]
        public string URLImagePhoto { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
