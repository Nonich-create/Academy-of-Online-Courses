﻿using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Students.BLL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ManagerService(UnitOfWork unitOfWork, ILogger<Manager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(Manager item)
        {
            try
            {
                await _unitOfWork.ManagerRepository.CreateAsync(item);
                await _unitOfWork.SaveAsync();
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
                Manager manager = await GetAsync(id);
                if (manager != null)
                {
                    await _unitOfWork.ApplicationUsers.DeleteAsync(manager.UserId);
                    await _unitOfWork.ManagerRepository.DeleteAsync(id);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Менеджер удален"); ;
                }
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
                return Enumerable.Empty<Manager>();
            }
        }

        public async Task<Manager> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение менеджера");
                return await _unitOfWork.ManagerRepository.GetAsync(id);
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
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Менеджер изменен");
                return manager;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования менаджер");
                return item;
            }
        }

        public async Task<Manager> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск менаджера");
                return await _unitOfWork.ManagerRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска менаджера");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.ManagerRepository.GetAllAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Manager>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.ManagerRepository.GetAllAsync())
                .OrderBy(t => t.Surname).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Manager>();
            return (await _unitOfWork.ManagerRepository.GetAllAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Manager>();
            return (await _unitOfWork.ManagerRepository.GetAllAsync()).AsQueryable()
                .OrderBy(t => t.Surname)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Manager>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }
}
