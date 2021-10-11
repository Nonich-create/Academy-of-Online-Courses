using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System.Linq;
using Students.BLL.Interface;
using Students.DAL.Specifications;

namespace Students.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UserService(UnitOfWork unitOfWork, ILogger<ApplicationUser> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            return await _unitOfWork.ApplicationUsersRepository.GetByIdAsync(id);
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
            try
            {
                _logger.LogInformation("Поиск пользователя");
                return await _unitOfWork.ApplicationUsersRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска пользователя");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                var spec = new UserWithItemsSpecifications();
                return (await _unitOfWork.ApplicationUsersRepository.CountAsync(spec));
            }
            var specSearch = new UserWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<ApplicationUser>();
            var spec = new UserWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(int currentPage, int pageSize, string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<ApplicationUser>();
            var spec = new UserWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.ApplicationUsersRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<ApplicationUser>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage, pageSize, searchString, searchParametr);
            }
            var spec = new UserWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.ApplicationUsersRepository.GetAsync(spec);
        }
    }

}
