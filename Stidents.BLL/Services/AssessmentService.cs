using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;

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
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлена в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
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

        public async Task<List<Assessment>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка оценок");
                return await _unitOfWork.AssessmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка оценок");
                return null;
            }
        }

        public async Task<List<Assessment>> GetAssessmentsByStudentId(int studentId)
        {
            var assessmentList = await _unitOfWork.AssessmentRepository.GetAllAsync();
            return (List<Assessment>)assessmentList.Where(x => x.StudentId == studentId);
        }

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

        public async Task Save() => await _unitOfWork.Save();

        public async Task<Assessment> Update(Assessment item)
        {
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.Update(item);
                _logger.LogInformation("Оценка изменена");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Оценка добавлена в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования оценки");
                return item;
            }
        }
    }
}
