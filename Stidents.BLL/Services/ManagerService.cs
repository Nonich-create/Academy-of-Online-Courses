using Microsoft.Extensions.Caching.Memory;
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

        public async Task<IEnumerable<Manager>> GetAllAsync()
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
                        cache.Set(manager.Id, manager,
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
        
        public async Task<Manager> Update(Manager item)
        {
            try
            {
                var manager = await _unitOfWork.ManagerRepository.Update(item);
                _logger.LogInformation("Менеджер изменен");
                return manager;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования менаджер");
                return item;
            }
        }

        public async Task<IEnumerable<Manager>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.ManagerRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<Manager> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск менаджера");
                return await _unitOfWork.ManagerRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска менаджера");
                return null;
            }
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.ManagerRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<Manager>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
