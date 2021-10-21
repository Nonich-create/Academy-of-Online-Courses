using Students.MVC.ViewModels.Base;
using System.ComponentModel.DataAnnotations;


namespace Students.MVC.ViewModels
{
    public class ChangePasswordViewModel : ModelReturnUrl
    {
        public string UserIdentityId { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
