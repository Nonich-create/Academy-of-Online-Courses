using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Students.DAL.Enum;

namespace Students.BLL.Services
{
    public class CourseApplicationService : ICourseApplicationService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public CourseApplicationService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<CourseApplication> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task Cancel(CourseApplication model)
        {
 
            var student = await _unitOfWork.StudentRepository.GetAsync(model.StudentId);
            student.GroupId = null;
            await _unitOfWork.StudentRepository.Update(student);
            model.ApplicationStatus = EnumApplicationStatus.Отменена;
            await _unitOfWork.ApplicationCourseRepository.Update(model);
            await _unitOfWork.Save();
        }

        public async Task CreateAsync(CourseApplication item)
        {
            try
            {
                await _unitOfWork.ApplicationCourseRepository.CreateAsync(item);
                _logger.LogInformation("Заявка создана");
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
                _logger.LogInformation(ex, "Ошибка создания заяки");
            }
        }


        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.ApplicationCourseRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Заяка удалена"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заяки");
            }
        }

        public async Task DeleteAsyncAll(int id)
        {
            try
            {
                await _unitOfWork.ApplicationCourseRepository.DeleteAsyncAll(id);
                _logger.LogInformation(id, "Заяки студента удалены"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заявок");
            }
        }
        public async Task Enroll(CourseApplication model) // посмотреть
        {
            var group =  ( await _unitOfWork.GroupRepository.GetAllAsync()).Where(g => g.CourseId == model.CourseId
            && g.CountMax > _unitOfWork.StudentRepository.GetAllAsync().Result.Where(s => s.GroupId == g.Id).Count()).First();
            if(group == null){ throw new InvalidOperationException($"На данный момент подходящих групп нет"); }
            var student = await _unitOfWork.StudentRepository.GetAsync(model.StudentId);
            if(student.GroupId != null) { throw new InvalidOperationException($"{student.Surname} {student.Name} {student.MiddleName} уже находится в группе"); }
            student.GroupId = group.Id;
            await _unitOfWork.StudentRepository.Update(student);
            model.ApplicationStatus = EnumApplicationStatus.Закрыта;
            await _unitOfWork.ApplicationCourseRepository.Update(model);
            await _unitOfWork.Save();
        }

        public async Task<bool> ExistsAsync(int id) => await _unitOfWork.ApplicationCourseRepository.ExistsAsync(id);


        public async Task<List<CourseApplication>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка заявок");
                return await _unitOfWork.ApplicationCourseRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка заявок");
                return null;
            }
        }


        public async Task<CourseApplication> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение заяки");
                if (!cache.TryGetValue(id, out CourseApplication courseApplication))
                {
                    _logger.LogInformation("Кэша нету");
                    courseApplication = await _unitOfWork.ApplicationCourseRepository.GetAsync(id);
                    if (courseApplication != null)
                    {
                        cache.Set(courseApplication.Id, courseApplication,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return courseApplication;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение заяки");
                return null;
            }
        }
        

        public async Task Save() => await _unitOfWork.Save();


        public async Task<CourseApplication> Update(CourseApplication item)
        {
            try
            {
                var courseApplication = await _unitOfWork.ApplicationCourseRepository.Update(item);
                _logger.LogInformation("Заявка изменена");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Оценка добавлена в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return courseApplication;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования заяки");
                return item;
            }
        }
        

    }
}
