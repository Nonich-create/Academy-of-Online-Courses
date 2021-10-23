using Students.DAL.Models;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    interface IEmailSenderService
    {
        Task SendAcceptanceConfirmation(string email, Group group, Student student);
    }
}
