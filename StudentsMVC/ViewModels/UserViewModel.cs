using Microsoft.AspNetCore.Identity;


namespace Students.MVC.ViewModels
{
    public class UserViewModel : IdentityUser
    {
        public string ReturnUrl { get; set; }
    }
}
