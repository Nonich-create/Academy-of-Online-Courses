using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.IO;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface ICourseService : IBaseService<Course>
    {
         Task<Stream> GetContent();
    }
}
