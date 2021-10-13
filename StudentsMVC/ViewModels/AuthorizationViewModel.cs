using Students.MVC.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class AuthorizationViewModel : ModelReturnUrl
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
