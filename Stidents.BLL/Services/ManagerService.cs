using Microsoft.Extensions.Logging;
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
                _logger.LogInformation($"Поиск менаджера {query}");;
                return await _unitOfWork.ManagerRepository.SearchAsync(query);
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation("Получение количество менеджеров");
            var specSearch = new ManagerWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Manager>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск менеджеров {searchString}"); ;
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Manager>();
            var spec = new ManagerWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Manager>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение менеджеров");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка менеджеров");
                return Enumerable.Empty<Manager>();
            }
            var spec = new ManagerWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.ManagerRepository.GetAsync(spec);
        }
    }
}
