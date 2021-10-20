using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    interface IEmailSenderService
    {
        Task SendAcceptanceConfirmation(string email, string text);
    }
}
