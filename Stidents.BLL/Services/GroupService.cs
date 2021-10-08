using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.Services
{

    public class GroupService : IGroupService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public GroupService(UnitOfWork unitOfWork, ILogger<Group> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(Group item)
        {
            try
            {
                await _unitOfWork.GroupRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();  
                _logger.LogInformation("Группа создана");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания группы");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Group group = await GetAsync(id);
                if (group != null)
                {
                    await _unitOfWork.GroupRepository.DeleteAsync(group);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Группа удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления группы");
            }
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка уроков");
                return await _unitOfWork.GroupRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка групп");
                return Enumerable.Empty<Group>();
            }
        }

        public async Task<Group> GetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }
                _logger.LogInformation("Получение группы");
                return await _unitOfWork.GroupRepository.GetByIdAsync((int)id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение группы");
                return null;
            }
        }

        public async Task<Group> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение группы");
                return await _unitOfWork.GroupRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение группы");
                return null;
            }
        }

        public async Task StartGroup(int id) // переделать на addRAnge
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(id);
                group.GroupStatus = EnumGroupStatus.Training;
                var students = (await _unitOfWork.StudentRepository.GetAllAsync()).Where(s => s.GroupId == group.Id);
                var lesson = (await _unitOfWork.LessonRepository.GetAllAsync()).Where(l => l.CourseId == group.CourseId);
                await _unitOfWork.GroupRepository.UpdateAsync(group);
                foreach (var itemLesson in lesson)
                {
                    foreach (var itemStudent in students)
                    {
                        await _unitOfWork.AssessmentRepository.AddAsync(new() { LessonId = itemLesson.Id, StudentId = itemStudent.Id, Score = 0 });
                    }
                    await _unitOfWork.LessonTimesRepository.AddAsync(new() { GroupId = group.Id, LessonId = itemLesson.Id });
                }
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Группа {group.Id} начала обучение");
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка старта группы");
            }
        }

        public async Task<Group> Update(Group item)
        {
            try
            {
                await _unitOfWork.GroupRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Группа изменена");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования группы");
                return item;
            }
        }

        public async Task<Group> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск группы");
                return await _unitOfWork.GroupRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска группы");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.GroupRepository.GetGroupsListAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Group>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.GroupRepository.GetGroupsListAsync())
                .OrderBy(g => g.NumberGroup).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Group>();
            return (await _unitOfWork.GroupRepository.GetGroupsListAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Group>();
            return (await _unitOfWork.GroupRepository.GetGroupsListAsync()).AsQueryable()
                .OrderBy(g => g.NumberGroup)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Group>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }

}
