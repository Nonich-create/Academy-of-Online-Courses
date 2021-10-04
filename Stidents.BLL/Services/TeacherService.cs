using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Students.DAL.Enum;

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
                await _unitOfWork.TeacherRepository.CreateAsync(item);
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
                await _unitOfWork.TeacherRepository.DeleteAsync(id);
                _logger.LogInformation(id, "Преподователь удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления преподавателя");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.TeacherRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка приподователей");
                return await _unitOfWork.TeacherRepository.GetAllAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка преподователей");
                return null;
            }
        }
         
        public async Task<Teacher> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение приподователя");
                return await _unitOfWork.TeacherRepository.GetAsync(id);
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
                var teacher = await _unitOfWork.TeacherRepository.Update(item);
                _logger.LogInformation("Преподователь изменен");
                return teacher;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования преподователя");
                return item;
            }
        }

        public async Task<IEnumerable<Teacher>>  GetAllTakeSkipAsync(int take, EnumPageActions action, int skip = 0)
        {
            return await _unitOfWork.TeacherRepository.GetAllTakeSkipAsync(take, action, skip);
        }

        public async Task<Teacher> SearchAsync(string predicate)
        {
            try
            {
                _logger.LogInformation("Поиск преподователя");
                return await _unitOfWork.TeacherRepository.SearchAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска преподователя");
                return null;
            }
        }

        public async Task<IEnumerable<Teacher>> SearchAllAsync(string searchString, EnumSearchParameters searchParameter, EnumPageActions action, int take, int skip = 0)
        {
            return await _unitOfWork.TeacherRepository.SearchAllAsync(searchString,searchParameter,action, take, skip);
        }

        public async Task<IEnumerable<Teacher>> DisplayingIndex(EnumPageActions action, string searchString, EnumSearchParameters searchParametr, int take, int skip = 0)
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
