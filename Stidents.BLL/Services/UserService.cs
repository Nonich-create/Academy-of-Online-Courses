using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System.Linq;

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
                return (await _unitOfWork.ApplicationUsersRepository.GetAllAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<ApplicationUser>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.ApplicationUsersRepository.GetAllAsync())
                .OrderBy(u => u.Email).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<ApplicationUser>();
            return (await _unitOfWork.ApplicationUsersRepository.GetAllAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<ApplicationUser>();
            return (await _unitOfWork.ApplicationUsersRepository.GetAllAsync()).AsQueryable()
                .OrderBy(u => u.Email)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<ApplicationUser>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }

}
