using Students.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
