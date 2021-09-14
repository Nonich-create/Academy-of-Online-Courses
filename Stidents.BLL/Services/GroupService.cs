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
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлена в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
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

        public async Task<List<Group>> GetAllAsync()
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

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task StartGroup(int id)
        {
            var group = await _unitOfWork.GroupRepository.GetAsync(id);
            group.GroupStatus = EnumGroupStatus.Обучение;
            var students = await _unitOfWork.StudentRepository.GetAllAsync();
            students = students.Where(s => s.GroupId == group.Id).ToList();
            var lesson = await _unitOfWork.LessonRepository.GetAllAsync();
            lesson = lesson.Where(l => l.CourseId == group.CourseId).ToList();
            await _unitOfWork.GroupRepository.Update(group);
            foreach (var itemLesson in lesson)
            {
                foreach (var itemStudent in students)
                {
                    Assessment assessment = new() { LessonId = itemLesson.Id, StudentId = itemStudent.Id };
                    await _unitOfWork.AssessmentRepository.CreateAsync(assessment);
                }
            }

            await _unitOfWork.Save();
        }

        public async Task<Group> Update(Group item)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.Update(item);
                _logger.LogInformation("Группа изменена");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Группа добавлена в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования группы");
                return item;
            }
        }
    }

}
