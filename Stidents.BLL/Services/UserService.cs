using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Students.BLL.Services
{
    public class UserService : IUserService
    {

         
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UnitOfWork unitOfWork, ILogger<ApplicationUser> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task CreateAsync(ApplicationUser item)
        {
            try
            {
                await _unitOfWork.ApplicationUsersRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Пользователь создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания пользователя");
            }
        }
        public async Task DeleteAsync(int id)
        {
            try
            {
                ApplicationUser applicationUser = await _unitOfWork.ApplicationUsersRepository.GetByIdAsync(id);
                if (applicationUser != null)
                {
                    await _unitOfWork.ApplicationUsersRepository.DeleteAsync(applicationUser);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Пользователь удален");
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex, "Ошибка удаления пользователя");
            }
        }
        public async Task DeleteAsync(string id)
        {
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    await _unitOfWork.ApplicationUsersRepository.DeleteAsync(applicationUser);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Пользователь удален");
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex, "Ошибка удаления пользователя");
            }
        }
        
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.ApplicationUsersRepository.GetAllAsync();
            }
            catch
            {
                return Enumerable.Empty<ApplicationUser>();
            }
        }

        public async Task<ApplicationUser> GetAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetAsync(int id)
        {
            return await _unitOfWork.ApplicationUsersRepository.GetByIdAsync(id);
        }

        public async Task<ApplicationUser> Update(ApplicationUser item)
        {
            try
            {
                await _unitOfWork.ApplicationUsersRepository.UpdateAsync(item);
                _logger.LogInformation("Пользователь изменен");
                await _unitOfWork.SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования пользователя");
                return item;
            }

        }

        public async Task<ApplicationUser> SearchAsync(string query)
        {
                _logger.LogInformation($"Поиск пользователей {query}");
                _logger.LogInformation("Поиск пользователя");
                return await _unitOfWork.ApplicationUsersRepository.SearchAsync(query);
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Получение количество пользователей");
            var specSearch = new UserWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск пользователей {searchString}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<ApplicationUser>();
            var spec = new UserWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<ApplicationUser>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение пользователей");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка пользователей");
                return Enumerable.Empty<ApplicationUser>();
            }
            var spec = new UserWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.GetAsync(spec);
        }
    }

}
