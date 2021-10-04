using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;

namespace Students.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CourseService(UnitOfWork unitOfWork, ILogger<Course> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(Course item)
        {
            try
            {
                await _unitOfWork.CourseRepository.CreateAsync(item);
                _logger.LogInformation("Курс создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания курсы");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.CourseRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Курс удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления курса");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.CourseRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка курсов");
                return await _unitOfWork.CourseRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка курсов");
                return null;
            }
        }

        public async Task<Course> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение курса");
                return await _unitOfWork.CourseRepository.GetAsync(id); 
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение курса");
                return null;
            }
        }

        public async Task<Course> Update(Course item)
        {
            try
            {
                var course = await _unitOfWork.CourseRepository.Update(item);
                _logger.LogInformation("Курс изменен");
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования курса");
                return item;
            }
        }

        public async Task<IEnumerable<Course>>GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.CourseRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<Course> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск курса");
                return await _unitOfWork.CourseRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска курса");
                return null;
            }
        }

        public async Task<IEnumerable<Course>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.CourseRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<Course>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
