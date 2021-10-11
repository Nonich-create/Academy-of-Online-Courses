using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq;
using System.Linq.Dynamic.Core;
using Students.BLL.Interface;
using Students.DAL.Specifications;

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
                await _unitOfWork.LessonTimesRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
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
                LessonTimes lessonTimes = await _unitOfWork.LessonTimesRepository.GetByIdAsync(id);
                if (lessonTimes != null)
                {
                    await _unitOfWork.LessonTimesRepository.DeleteAsync(lessonTimes);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Время занятия удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления времени занятия");
            }
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
                return Enumerable.Empty<LessonTimes>();
            }
        }

        public async Task<LessonTimes> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение времени проведния занятия");
                return await _unitOfWork.LessonTimesRepository.GetByIdAsync(id);
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
                await _unitOfWork.LessonTimesRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Время проведения занятия изменено");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования времени проведения занятия");
                return item;
            }
        }

        public async Task<LessonTimes> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск времени проведения занятия");
                return await _unitOfWork.LessonTimesRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска времени проведения занятия");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                var spec = new LessonTimesWithItemsSpecifications();
                return (await _unitOfWork.LessonTimesRepository.CountAsync(spec));
            }
            var specSearch = new LessonTimesWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.LessonTimesRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<LessonTimes>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<LessonTimes>();
            var spec = new LessonTimesWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.LessonTimesRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<LessonTimes>> SearchAllAsync(int currentPage, int pageSize, string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<LessonTimes>();
            var spec = new LessonTimesWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.LessonTimesRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<LessonTimes>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage, pageSize, searchString, searchParametr);
            }
            var spec = new LessonTimesWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.LessonTimesRepository.GetAsync(spec);
        }
    }
}
 