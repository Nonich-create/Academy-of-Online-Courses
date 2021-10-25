using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System.Linq;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Students.BLL.Services
{
    public class TeacherService : ITeacherService
    {

        private readonly ILogger _logger;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherService(UnitOfWork unitOfWork, ILogger<Teacher> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }
        
        public async Task CreateAsync(Teacher item)
        {
            try
            {
                await _unitOfWork.TeacherRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Преподователь создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания преподователя");
            }
        }

        public async Task CreateAsync(Teacher teacher, ApplicationUser user, string password)
        {

            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "teacher");
                    teacher.UserId = user.Id;
                    await _unitOfWork.TeacherRepository.AddAsync(teacher);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Преподователь создан");
                }
                else
                {
                    _logger.LogInformation("Ошибка создания пользователя");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания преподователя");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Teacher teacher = await GetAsync(id);
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(teacher.UserId);
                if (teacher != null)
                {
                    await _unitOfWork.ApplicationUsersRepository.DeleteAsync(applicationUser);
                    await _unitOfWork.TeacherRepository.DeleteAsync(teacher);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Преподователь удален");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления преподавателя");
            }
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка преподователей");
                return await _unitOfWork.TeacherRepository.GetAllAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка преподователей");
                return Enumerable.Empty<Teacher>();
            }
        }
         
        public async Task<Teacher> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение преподователя");
                return await _unitOfWork.TeacherRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение преподователя");
                return null;
            }
        }

   
         
        public async Task<Teacher> Update(Teacher item)
        {
            try
            {
                await _unitOfWork.TeacherRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Преподователь изменен");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования преподователя");
                return item;
            }
        }

        public async Task<Teacher> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск преподователя");
                return await _unitOfWork.TeacherRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска преподователя");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                var spec = new TeacherWithItemsSpecifications();
                return (await _unitOfWork.TeacherRepository.CountAsync(spec));
            }
            var specSearch = new TeacherWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.TeacherRepository.CountAsync(specSearch);
        }
        
        public async Task<IEnumerable<Teacher>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Teacher>();
            var spec = new TeacherWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.TeacherRepository.GetAsync(spec);

        }

        public async Task<IEnumerable<Teacher>> SearchAllAsync(int currentPage, int pageSize, string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Teacher>();
            var spec = new TeacherWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.TeacherRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Teacher>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage, pageSize, searchString, searchParametr);
            }
            var spec = new TeacherWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.TeacherRepository.GetAsync(spec);
        }
    }
}
