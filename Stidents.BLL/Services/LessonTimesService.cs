using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public class LessonTimesService : ILessonTimesService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public LessonTimesService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<LessonTimes> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(LessonTimes item)
        {
            try
            {
                await _unitOfWork.LessonTimesRepository.CreateAsync(item);
                int n = await _unitOfWork.Save();
                _logger.LogInformation("Время урока добавлена");
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
                if (!cache.TryGetValue(id, out LessonTimes lesson))
                {
                    _logger.LogInformation("Кэша нету");
                    lesson = await _unitOfWork.LessonTimesRepository.GetAsync(id);
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
                _logger.LogInformation(ex, "Ошибка при получение времени проведения занятия");
                return null;
            }
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<LessonTimes> Update(LessonTimes item)
        {
            try
            {
                var lessonTimes = await _unitOfWork.LessonTimesRepository.Update(item);
                int n = await _unitOfWork.Save();
                _logger.LogInformation("Время проведения занятия изменено");
                if (n > 0)
                {
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return lessonTimes;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования времени проведения занятия");
                return item;
            }
        }
    }
}
 