using System;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
