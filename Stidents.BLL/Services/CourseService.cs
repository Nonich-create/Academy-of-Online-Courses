using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq;
using Students.BLL.GenerateFile;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using System.IO;
using System.Text;

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

        public async Task<Stream> GetContent()
        {
            var courses = await _unitOfWork.CourseRepository.GetAllAsync();
            var sb = new StringBuilder();
            GenerateStream generateFile = new();

            foreach (var course in courses)
            {
                sb.AppendLine($"{course.Name};{course.Description};{course.Price}");
            }
 
            return generateFile.GenerateStreamFromString(sb.ToString());
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
            _logger.LogInformation("Получение количество курсов");
            var specSearch = new CourseWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.CourseRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Course>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск курсов {searchString}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Course>();
            var spec = new CourseWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.CourseRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Course>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение курсов");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка получение курсов");
                return Enumerable.Empty<Course>();
            }
            var spec = new CourseWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.CourseRepository.GetAsync(spec);
        }
    }
}
