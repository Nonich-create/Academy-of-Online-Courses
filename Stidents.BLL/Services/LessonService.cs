using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Students.DAL.Enum;
using Students.BLL.Interface;
using Students.DAL.Specifications;

namespace Students.BLL.Services
{
    public class LessonService : ILessonService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public LessonService(UnitOfWork unitOfWork, ILogger<Lesson> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        public async Task<bool> CheckRecord(int CourseId, int NumberLesson)
        {
               if (await _unitOfWork.LessonRepository.SearchAsync($"CourseId = {CourseId} and NumberLesson = {NumberLesson}") == null)
            return true;
            return false;
        }
        
        public async Task CreateAsync(Lesson item)
        {
            try 
            {
                await _unitOfWork.LessonRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Урок создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания урока");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Lesson lesson = await _unitOfWork.LessonRepository.GetByIdAsync(id);
                if (lesson != null)
                {
                    await _unitOfWork.LessonRepository.DeleteAsync(lesson);
                    _logger.LogInformation(id, "Урок удален");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления урока");
            }
        }
        
        public async Task<IEnumerable<Lesson>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка уроков");
                return await _unitOfWork.LessonRepository.GetAllAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка уроков");
                return Enumerable.Empty<Lesson>();
            }
        }

        public async Task<Lesson> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение урока");
                var spec = new LessonWithItemsSpecifications((uint)id);
                return await _unitOfWork.LessonRepository.GetAsync(spec,true); 
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение урока");
                return null;
            }
        }

        public async Task<Lesson> Update(Lesson item)
        {
            try
            {
                await _unitOfWork.LessonRepository.UpdateAsync(item);
                _logger.LogInformation("Урок изменен");
                await _unitOfWork.SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования урока");
                return item;
            }
        }

        public async Task<Lesson> SearchAsync(string query)
        {
                _logger.LogInformation($"Поиск урока {query}");
                return await _unitOfWork.LessonRepository.SearchAsync(query);
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Полученик количество уроков");
            var specSearch = new LessonWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.LessonRepository.CountAsync(specSearch);
        }

        public async Task<int> GetCount(int courseId, string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Полученик количество уроков для курса {courseId}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                var spec = new LessonWithItemsSpecifications(courseId);
                return (await _unitOfWork.LessonRepository.CountAsync(spec));
            }
            var specSearch = new LessonWithItemsSpecifications(courseId, searchString, searchParametr);
            return await _unitOfWork.LessonRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Lesson>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск урока {searchString}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Lesson>();
            var spec = new LessonWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.LessonRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Lesson>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение занятий");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка получение занятий");
                return Enumerable.Empty<Lesson>();
            }
            var spec = new LessonWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.LessonRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Lesson>> IndexView(int courseId, string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение занятий");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка получение занятий");
                return Enumerable.Empty<Lesson>();
            }
            var specSearch = new LessonWithItemsSpecifications(currentPage, pageSize, searchString, courseId, searchParametr);
            return await _unitOfWork.LessonRepository.GetAsync(specSearch);
        }
    }
}
