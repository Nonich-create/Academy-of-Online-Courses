using Students.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Students.MVC.ViewModels
{
    public class PersonViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указан Email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public  string Email { get; set; }

        [Required(ErrorMessage = "Не указан номер телефона")]
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
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

        [Display(Name = "ФИО")]
        public string GetFullName => $"{Surname} {Name} {MiddleName}";
    }
}
