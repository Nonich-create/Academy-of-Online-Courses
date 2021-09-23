using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Students.DAL.Enum;

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
                await _unitOfWork.ApplicationUsers.CreateAsync(item);
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
                await _unitOfWork.ApplicationUsers.DeleteAsync(id);
                _logger.LogInformation("Пользователь удален");
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex, "Ошибка удаления пользователя");
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
                return await GetAsync(id) != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.ApplicationUsers.ExistsAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _unitOfWork.ApplicationUsers.GetAllAsync();
        }

        public async Task<ApplicationUser> GetAsync(string id)
        {
            return await _unitOfWork.ApplicationUsers.GetAsync(id);
        }

        public async Task<ApplicationUser> GetAsync(int id)
        {
            return await _unitOfWork.ApplicationUsers.GetAsync(id);
        }

        public async Task<ApplicationUser> Update(ApplicationUser item)
        {
            return await _unitOfWork.ApplicationUsers.Update(item);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.ApplicationUsers.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.ApplicationUsers.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<ApplicationUser>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
