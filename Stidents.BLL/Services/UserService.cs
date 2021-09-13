using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Microsoft.Extensions.Logging;
using System;

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

        public async Task<List<ApplicationUser>> GetAllAsync()
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

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task<ApplicationUser> Update(ApplicationUser item)
        {
            return await _unitOfWork.ApplicationUsers.Update(item);
        }
    }

}
