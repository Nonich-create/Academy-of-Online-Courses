using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq;
using System.Linq.Dynamic.Core;
using Students.BLL.Interface;

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
                await _unitOfWork.CourseRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
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
                Course course = await GetAsync(id);
                if (course != null)
                {
                    await _unitOfWork.CourseRepository.DeleteAsync(course);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Курс удален"); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления курса");
            }
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
                return Enumerable.Empty<Course>();
            }
        }

        public async Task<Course> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение курса");
                return await _unitOfWork.CourseRepository.GetByIdAsync(id); 
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
                await _unitOfWork.CourseRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Курс изменен");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования курса");
                return item;
            }
        }

        public async Task<Course> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск курса");
                return await _unitOfWork.CourseRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска курса");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.CourseRepository.GetAllAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Course>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.CourseRepository.GetAllAsync())
                .OrderBy(c => c.Name).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Course>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Course>();
            return (await _unitOfWork.CourseRepository.GetAllAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Course>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Course>();
            return (await _unitOfWork.CourseRepository.GetAllAsync()).AsQueryable()
                .OrderBy(c => c.Name)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Course>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
