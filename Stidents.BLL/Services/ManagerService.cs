using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.BLL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public ManagerService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Manager> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task CreateAsync(Manager item)
        {
            try
            {
                await _unitOfWork.ManagerRepository.CreateAsync(item);
                _logger.LogInformation("Менеджер создан");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлен в кэш");
                    cache.Set(item.ManagerId, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания менеджера");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _unitOfWork.ManagerRepository.DeleteAsync(id); ;
                _logger.LogInformation(id, "Менеджер удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления менеджера");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.ManagerRepository.ExistsAsync(id);
        }

        public async Task<List<Manager>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка менеджеров");
                return await _unitOfWork.ManagerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка менеджеров");
                return null;
            }
        }

        public async Task<Manager> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение менеджера");
                if (!cache.TryGetValue(id, out Manager manager))
                {
                    _logger.LogInformation("Кэша нету");
                    manager = await _unitOfWork.ManagerRepository.GetAsync(id);
                    if (manager != null)
                    {
                        cache.Set(manager.ManagerId, manager,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return manager;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение менеджера");
                return null;
            }
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<Manager> Update(Manager item)
        {
            try
            {
                var manager = await _unitOfWork.ManagerRepository.Update(item);
                _logger.LogInformation("Менеджер изменен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Менаджер добавлен в кэш");
                    cache.Set(item.ManagerId, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return manager;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования менаджер");
                return item;
            }
        }
    }
}
