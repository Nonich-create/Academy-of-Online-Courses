using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Students.MVC.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
        public IList<string> UserRoles { get; set; } = new List<string>();
    }
}
