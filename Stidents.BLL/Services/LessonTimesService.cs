using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.DAL.Enum;

namespace Students.BLL.Services
{
    public class LessonTimesService : ILessonTimesService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public LessonTimesService(UnitOfWork unitOfWork, ILogger<LessonTimes> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(LessonTimes item)
        {
            try
            {
                await _unitOfWork.LessonTimesRepository.CreateAsync(item);
                _logger.LogInformation("Время урока добавлена");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания времени урока");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.LessonTimesRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Время занятия удалена"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления времени занятия");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {

            return await _unitOfWork.LessonTimesRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<LessonTimes>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка времени проведния занятий");
                return await _unitOfWork.LessonTimesRepository.GetAllAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка времени проведния занятий");
                return null;
            }
        }

        public async Task<LessonTimes> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение времени проведния занятия");
                return await _unitOfWork.LessonTimesRepository.GetAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение времени проведения занятия");
                return null;
            }
        }

        public async Task<LessonTimes> Update(LessonTimes item)
        {
            try
            {
                var lessonTimes = await _unitOfWork.LessonTimesRepository.Update(item);
                _logger.LogInformation("Время проведения занятия изменено");
                return lessonTimes;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования времени проведения занятия");
                return item;
            }
        }

        public async Task<IEnumerable<LessonTimes>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.LessonTimesRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<LessonTimes> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск времени проведения занятия");
                return await _unitOfWork.LessonTimesRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска времени проведения занятия");
                return null;
            }
        }

        public async Task<IEnumerable<LessonTimes>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.LessonTimesRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<LessonTimes>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
 