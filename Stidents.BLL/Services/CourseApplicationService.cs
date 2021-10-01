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
            var group =  (await _unitOfWork.GroupRepository.GetAllAsync()).First(g => g.CourseId == model.CourseId
            && g.Id == (int)student.GroupId 
            && g.GroupStatus == EnumGroupStatus.Training);
            if (group == null)
            {
                student.GroupId = null;
                await _unitOfWork.StudentRepository.Update(student);
                model.ApplicationStatus = EnumApplicationStatus.Cancelled;
                await _unitOfWork.CourseApplicationRepository.Update(model);
                _logger.LogInformation($"Заявка студента {student.Id} на курс {model.CourseId} отменена");
            }
            else 
            {
                _logger.LogInformation($"На данный момент студент {student.Id} обучается в группе {group.Id}");
                throw new InvalidOperationException($"На данный момент студент {student.Id} обучается в группе {group.Id}");
            }
        }

        public async Task CreateAsync(CourseApplication item)
        {
            try
            {
                await _unitOfWork.CourseApplicationRepository.CreateAsync(item);   
                _logger.LogInformation("Заявка создана");
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
                await _unitOfWork.CourseApplicationRepository.DeleteAsync(id);
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
                await _unitOfWork.CourseApplicationRepository.DeleteAsyncAll(id);
                _logger.LogInformation(id, "Заяки студента удалены"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заявок");
            }
        }
        public async Task Enroll(CourseApplication model) 
        {
            var students = (await _unitOfWork.StudentRepository.GetAllAsync()).Where(s => s.GroupId != null );
            var group =  (await _unitOfWork.GroupRepository.GetAllAsync()).First(g => g.CourseId == model.CourseId && 
            g.GroupStatus == EnumGroupStatus.Set &&
            g.CountMax > students.Count(s =>s.GroupId == g.Id));
            if (group == null){ throw new InvalidOperationException($"На данный момент подходящих групп нет"); }
            var student = await _unitOfWork.StudentRepository.GetAsync(model.StudentId);
            if(student.GroupId != null) { throw new InvalidOperationException($"{student.Surname} {student.Name} {student.MiddleName} уже находится в группе"); }
            student.GroupId = group.Id;
            await _unitOfWork.StudentRepository.Update(student);
            model.ApplicationStatus = EnumApplicationStatus.Close;
            await _unitOfWork.CourseApplicationRepository.Update(model);
            _logger.LogInformation($"Студент {model.StudentId} зачислен в группу {group.Id}"); ;
        }

        public async Task<bool> ExistsAsync(int id) => await _unitOfWork.CourseApplicationRepository.ExistsAsync(id);


        public async Task<IEnumerable<CourseApplication>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка заявок");
                return await _unitOfWork.CourseApplicationRepository.GetAllAsync();
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
                    courseApplication = await _unitOfWork.CourseApplicationRepository.GetAsync(id);
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
       
        public async Task<CourseApplication> Update(CourseApplication item)
        {
            try
            {
                var courseApplication = await _unitOfWork.CourseApplicationRepository.Update(item);
                _logger.LogInformation("Заявка изменена");
                return courseApplication;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования заяки");
                return item;
            }
        }

        public async Task<IEnumerable<CourseApplication>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.CourseApplicationRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<CourseApplication> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск заяки");
                return await _unitOfWork.CourseApplicationRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска заяки");
                return null;
            }
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.CourseApplicationRepository.SearchAllAsync(searchString, searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<CourseApplication>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
