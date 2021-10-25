﻿using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Students.BLL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerService(UnitOfWork unitOfWork, ILogger<Manager> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task CreateAsync(Manager item)
        {
            try
            {
                await _unitOfWork.ManagerRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Менеджер создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания менеджера");
            }
        }

        public async Task CreateAsync(Manager manager, ApplicationUser user, string password)
        {

            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "manager");
                    manager.UserId = user.Id;
                    await _unitOfWork.ManagerRepository.AddAsync(manager);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Менеджер создан");
                }
                else
                {
                    _logger.LogInformation("Ошибка создания пользователя");
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
                Manager manager = await _unitOfWork.ManagerRepository.GetByIdAsync(id);
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(manager.UserId);
                if (manager != null)
                {
                    await _unitOfWork.ApplicationUsersRepository.DeleteAsync(applicationUser);
                    await _unitOfWork.ManagerRepository.DeleteAsync(manager);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Менеджер удален"); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления менеджера");
            }
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
                return await _unitOfWork.ManagerRepository.GetByIdAsync(id);
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
                await _unitOfWork.ManagerRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Менеджер изменен");
                return item;
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
                var spec = new ManagerWithItemsSpecifications();
                return (await _unitOfWork.ManagerRepository.CountAsync(spec));
            }
            var specSearch = new ManagerWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Manager>();
            var spec = new ManagerWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(int currentPage, int pageSize, string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Manager>();
            var spec = new ManagerWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Manager>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage, pageSize, searchString, searchParametr);
            }
            var spec = new ManagerWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.ManagerRepository.GetAsync(spec);
        }
    }
}
