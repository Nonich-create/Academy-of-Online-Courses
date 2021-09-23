using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using Students.DAL.Enum;

namespace Students.BLL.Services
{

    public class AssessmentService : IAssessmentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public AssessmentService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Assessment> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(Assessment item)
        {
            try
            {
                await _unitOfWork.AssessmentRepository.CreateAsync(item);
                _logger.LogInformation("Оценка создана");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания оценки");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.AssessmentRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Оценка удалена"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления оценки");
            }
        }

        public async Task<bool> ExistsAsync(int id) => await _unitOfWork.AssessmentRepository.ExistsAsync(id);

        public async Task<IEnumerable<Assessment>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка оценок");
                return await  _unitOfWork.AssessmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка оценок");
                return null;
            }
        }

   

        public async Task<IEnumerable<Assessment>> GetAssessmentsByStudentId(int studentId)
         => (await _unitOfWork.AssessmentRepository.GetAllAsync()).Where(x => x.StudentId == studentId);
 
        

        public async Task<Assessment> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение оценки");
                if (!cache.TryGetValue(id, out Assessment assessment))
                {
                    _logger.LogInformation("Кэша нету");
                    assessment = await _unitOfWork.AssessmentRepository.GetAsync(id);
                    if (assessment != null)
                    {
                        cache.Set(assessment.Id, assessment,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение оценки");
                return null;
            }
        }

        public async Task<Assessment> Update(Assessment item)
        {
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.Update(item);
                _logger.LogInformation("Оценка изменена");
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования оценки");
                return item;
            }
        }

        public async Task<IEnumerable<Assessment>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
           return await _unitOfWork.AssessmentRepository.GetAllTakeSkipAsync(take,action, skip);
        }

        public async Task<IEnumerable<Assessment>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.AssessmentRepository.SearchAllAsync(searchString, searchParametr,action, take, skip);
        }

        public async Task<IEnumerable<Assessment>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
