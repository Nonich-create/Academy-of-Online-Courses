using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;

namespace Students.BLL.Services
{

    public class GroupService : IGroupService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public GroupService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Group> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(Group item)
        {
            try
            {
                await _unitOfWork.GroupRepository.CreateAsync(item);
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
                await _unitOfWork.GroupRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Группа удалена"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления группы");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.GroupRepository.ExistsAsync(id);
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
                return null;
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
                if (!cache.TryGetValue(id, out Group group))
                {
                    _logger.LogInformation("Кэша нету");
                    group = await _unitOfWork.GroupRepository.GetAsync(id);
                    if (group != null)
                    {
                        cache.Set(group.Id, group,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return group;
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
                if (!cache.TryGetValue(id, out Group group))
                {
                    _logger.LogInformation("Кэша нету");
                    group = await _unitOfWork.GroupRepository.GetAsync(id);
                    if (group != null)
                    {
                        cache.Set(group.Id, group,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение группы");
                return null;
            }
        }

        public async Task StartGroup(int id)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetAsync(id);
                group.GroupStatus = EnumGroupStatus.Training;
                var students = (await _unitOfWork.StudentRepository.GetAllAsync()).Where(s => s.GroupId == group.Id);
                var lesson = (await _unitOfWork.LessonRepository.GetAllAsync()).Where(l => l.CourseId == group.CourseId);
                await _unitOfWork.GroupRepository.Update(group);
                foreach (var itemLesson in lesson)
                {
                    foreach (var itemStudent in students)
                    {
                        await _unitOfWork.AssessmentRepository.CreateAsync(new() { LessonId = itemLesson.Id, StudentId = itemStudent.Id, Score = 0 });
                    }
                    await _unitOfWork.LessonTimesRepository.CreateAsync(new() { GroupId = group.Id, LessonId = itemLesson.Id });
                }
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
                var group = await _unitOfWork.GroupRepository.Update(item);
                _logger.LogInformation("Группа изменена");
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования группы");
                return item;
            }
        }

        public async Task<IEnumerable<Group>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.GroupRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<Group> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск группы");
                return await _unitOfWork.GroupRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска группы");
                return null;
            }
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.GroupRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<Group>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
        {
            take = (take == 0) ? 10 : take;
            if (!String.IsNullOrEmpty(searchString))
            {
                return await SearchAllAsync(searchString, searchParametr, action, take, skip);
            }
            return await GetAllTakeSkipAsync(take, action, skip);
        }
    }

}
