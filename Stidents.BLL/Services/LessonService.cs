using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Services
{

    public class LessonService : ILessonService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public LessonService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Lesson> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(Lesson item)
        {
            try
            {
                await _unitOfWork.LessonRepository.CreateAsync(item);
                _logger.LogInformation("Урок создан");
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

        public async Task<List<Lesson>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка уроков");
                List<Lesson> lessons = null;
                lessons = await _unitOfWork.LessonRepository.GetAllAsync();
                return lessons;
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
                if (!cache.TryGetValue(id, out Lesson lesson))
                {     
                    _logger.LogInformation("Кэша нету");
                    lesson = await _unitOfWork.LessonRepository.GetAsync(id);
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
                _logger.LogInformation(ex, "Ошибка при получение урока");
                return null;
            }
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<Lesson> Update(Lesson item)
        {
            try
            {
                var lesson = await _unitOfWork.LessonRepository.Update(item);
                _logger.LogInformation("Урок изменен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Урок добавлен в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования урока");
                return item;
            }
        }
    }
}
