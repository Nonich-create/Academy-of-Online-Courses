using Students.BLL.Interface.Base;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Interface
{
    public interface IGroupService : IBaseService<Group>
    {
        Task<Group> GetAsync(int? id);
        Task StartGroup(int id);
        Task FinallyGroup(int groupId);
        Task CancelGroup(int groupId);
        Task RefreshGroup(int groupId);
        Task<IEnumerable<Group>> SearchAllAsync(string query);
        Task<int> GetCount(int teacherId);
        Task<IEnumerable<Group>> IndexView(int teacherId, int currentPage, int pageSize = 10);
    }
}
