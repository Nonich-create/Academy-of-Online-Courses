using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public LessonPlanService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<LessonPlan> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(LessonPlan item)
        {
            try
            {
                await _unitOfWork.LessonPlanRepository.CreateAsync(item);
                _logger.LogInformation("План добавлен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлен в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания плана");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.LessonPlanRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Урок план"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления плана");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {

            return await _unitOfWork.LessonPlanRepository.ExistsAsync(id);
        }

        public async Task<List<LessonPlan>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка планов");
                List<LessonPlan> lessons = null;
                lessons = await _unitOfWork.LessonPlanRepository.GetAllAsync();
                return lessons;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка планов");
                return null;
            }
        }

        public async Task<LessonPlan> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение плана");
                if (!cache.TryGetValue(id, out LessonPlan lesson))
                {
                    _logger.LogInformation("Кэша нету");
                    lesson = await _unitOfWork.LessonPlanRepository.GetAsync(id);
                    if (lesson != null)
                    {
                        cache.Set(lesson.Id, lesson,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение плана");
                return null;
            }
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<LessonPlan> Update(LessonPlan item)
        {
            try
            {
                var lessonPlan = await _unitOfWork.LessonPlanRepository.Update(item);
                _logger.LogInformation("План изменен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("План добавлен в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return lessonPlan;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования плана");
                return item;
            }
        }
    }
}
