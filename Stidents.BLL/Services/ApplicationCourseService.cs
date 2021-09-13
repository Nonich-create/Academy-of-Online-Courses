using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public class ApplicationCourseService: IApplicationCourseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public ApplicationCourseService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<ApplicationCourse> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task Cancel(ApplicationCourse model)
        {
 
            var student = await _unitOfWork.StudentRepository.GetAsync(model.StudentId);
            student.GroupId = null;
            await _unitOfWork.StudentRepository.Update(student);
            model.ApplicationStatus = Enum.EnumApplicationStatus.Отменена.ToString();
            await _unitOfWork.ApplicationCourseRepository.Update(model);
            await _unitOfWork.Save();
        }

        public async Task CreateAsync(ApplicationCourse item)
        {
            try
            {
                await _unitOfWork.ApplicationCourseRepository.CreateAsync(item);
                _logger.LogInformation("Заявка создана");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлена в кэш");
                    cache.Set(item.ApplicationCourseId, item, new MemoryCacheEntryOptions
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
        public async Task Enroll(ApplicationCourse model)
        {
            var group =  _unitOfWork.GroupRepository.GetAllAsync().Result.Where(g => g.CourseId == model.CourseId
            && g.CountMax > _unitOfWork.StudentRepository.GetAllAsync().Result.Where(s => s.GroupId == g.GroupId).Count()).First();
            if(group == null){ throw new InvalidOperationException($"На данный момент подходящих групп нет"); }
            var student = await _unitOfWork.StudentRepository.GetAsync(model.StudentId);
            if(student.GroupId != null) { throw new InvalidOperationException($"{student.Surname} {student.Name} {student.MiddleName} уже находится в группе"); }
            student.GroupId = group.GroupId;
            await _unitOfWork.StudentRepository.Update(student);
            model.ApplicationStatus = Enum.EnumApplicationStatus.Закрыта.ToString();
            await _unitOfWork.ApplicationCourseRepository.Update(model);
            await _unitOfWork.Save();
        }

        public async Task<bool> ExistsAsync(int id) => await _unitOfWork.ApplicationCourseRepository.ExistsAsync(id);


        public async Task<List<ApplicationCourse>> GetAllAsync()
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


        public async Task<ApplicationCourse> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение заяки");
                if (!cache.TryGetValue(id, out ApplicationCourse applicationCourse))
                {
                    _logger.LogInformation("Кэша нету");
                    applicationCourse = await _unitOfWork.ApplicationCourseRepository.GetAsync(id);
                    if (applicationCourse != null)
                    {
                        cache.Set(applicationCourse.ApplicationCourseId, applicationCourse,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return applicationCourse;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение заяки");
                return null;
            }
        }
        

        public async Task Save() => await _unitOfWork.Save();


        public async Task<ApplicationCourse> Update(ApplicationCourse item)
        {
            try
            {
                var applicationCourse = await _unitOfWork.ApplicationCourseRepository.Update(item);
                _logger.LogInformation("Заявка изменена");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Оценка добавлена в кэш");
                    cache.Set(item.ApplicationCourseId, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return applicationCourse;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования заяки");
                return item;
            }
        }
        

    }
}
