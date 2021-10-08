using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Students.BLL.Services
{
    public class TeacherService : ITeacherService
    {

        private readonly ILogger _logger;
        private readonly UnitOfWork _unitOfWork;

        public TeacherService(UnitOfWork unitOfWork, ILogger<Teacher> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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

        public async Task DeleteAsync(int id)
        {
            try
            {
                Teacher teacher = await GetAsync(id);
                ApplicationUser applicationUser = await _unitOfWork.ApplicationUsersRepository.GetByIdAsync(teacher.UserId);
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
                return (await _unitOfWork.TeacherRepository.GetTeacherListAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Teacher>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.TeacherRepository.GetTeacherListAsync())
                .OrderBy(t => t.Surname).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Teacher>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Teacher>();
            return (await _unitOfWork.TeacherRepository.GetTeacherListAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Teacher>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Teacher>();
            return (await _unitOfWork.TeacherRepository.GetTeacherListAsync()).AsQueryable()
                .OrderBy(t => t.Surname)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Teacher>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }
}
