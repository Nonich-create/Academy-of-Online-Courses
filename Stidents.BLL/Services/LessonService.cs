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
                await _unitOfWork.LessonRepository.CreateAsync(item);
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
                await _unitOfWork.LessonRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Урок удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления урока");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.LessonRepository.ExistsAsync(id);
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
                return null;
            }
        }

        public async Task<Lesson> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение урока");
                return await _unitOfWork.LessonRepository.GetAsync(id); 
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
                var lesson = await _unitOfWork.LessonRepository.Update(item);
                _logger.LogInformation("Урок изменен");
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования урока");
                return item;
            }
        }

        public async Task<IEnumerable<Lesson>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.LessonRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<Lesson> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск урока");
                return await _unitOfWork.LessonRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска урока");
                return null;
            }
        }

        public async Task<IEnumerable<Lesson>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.LessonRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<Lesson>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
        {
            take = (take == 0) ? 10 : take;
            if (!String.IsNullOrEmpty(searchString))
            {
                return await SearchAllAsync(searchString, searchParametr, action, take, skip);
            }
            return await GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<IEnumerable<Lesson>> DisplayingIndexByIdCourse(int id,EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
        {
            take = (take == 0) ? 10 : take;
            if (!String.IsNullOrEmpty(searchString))
            {
                return (await SearchAllAsync(searchString, searchParametr, action, take, skip)).Where(l => l.CourseId == id);
            }
            return (await GetAllTakeSkipAsync(take, action, skip)).Where(l => l.CourseId == id);
        }
    }
}
