using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;

namespace Students.BLL.Services
{
    public class TeacherService : ITeacherService
    {

        private readonly IMemoryCache cache;
        private readonly ILogger _logger;
        private readonly UnitOfWork _unitOfWork;

        public TeacherService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Teacher> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(Teacher item)
        {
            try
            {
                await _unitOfWork.TeacherRepository.CreateAsync(item);
                _logger.LogInformation("Преподователь создан");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлен в кэш");
                    cache.Set(item.TeacherId, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex, "Ошибка создания преподователя");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.TeacherRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Преподователь удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления приподавателя");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.TeacherRepository.ExistsAsync(id);
        }

        public async Task<List<Teacher>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка приподователей");
                return await _unitOfWork.TeacherRepository.GetAllAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка приподователей");
                return null;
            }
        }

        public async Task<Teacher> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение приподователя");
                if (!cache.TryGetValue(id, out Teacher teacher))
                {
                    _logger.LogInformation("Кэша нету");
                    teacher = await _unitOfWork.TeacherRepository.GetAsync(id);
                    if (teacher != null)
                    {
                        cache.Set(teacher.TeacherId, teacher,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return teacher;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение приподователя");
                return null;
            }
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<Teacher> Update(Teacher item)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepository.Update(item);
                _logger.LogInformation("Преподователь изменен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Преподователь добавлен в кэш");
                    cache.Set(item.TeacherId, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return teacher;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования приподователя");
                return item;
            }
        }
    }
}
