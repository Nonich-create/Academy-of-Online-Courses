using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;

namespace Students.BLL.Services
{

    public class AssessmentService : IAssessmentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AssessmentService(UnitOfWork unitOfWork, ILogger<Assessment> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(Assessment item)
        {
            try
            {
                await _unitOfWork.AssessmentRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Оценка создана");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания оценки");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Assessment assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(id);
                if (assessment != null)
                {
                    await _unitOfWork.AssessmentRepository.DeleteAsync(assessment);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Оценка удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления оценки");
            }
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка оценок");
                return await  _unitOfWork.AssessmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка оценок");
                return Enumerable.Empty<Assessment>();
            }
        }

        public async Task<Assessment> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение оценки");
                return await _unitOfWork.AssessmentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение оценки");
                return null;
            }
        }

        public async Task<Assessment> Update(Assessment item)
        {
            try
            {
                await _unitOfWork.AssessmentRepository.UpdateAsync(item);
                _logger.LogInformation("Оценка изменена");
                await _unitOfWork.SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования оценки");
                return item;
            }
        }

        public async Task<Assessment> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск оценки");
                return await _unitOfWork.AssessmentRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска оценки");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.AssessmentRepository.GetAssessmentsListAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Assessment>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.AssessmentRepository.GetAssessmentsListAsync())
                .OrderBy(a => a.Lesson.NumberLesson).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Assessment>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Assessment>();
            return (await _unitOfWork.AssessmentRepository.GetAssessmentsListAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Assessment>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Assessment>();
            return (await _unitOfWork.AssessmentRepository.GetAssessmentsListAsync()).AsQueryable()
                .OrderBy(a => a.Lesson.NumberLesson)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Assessment>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }
}
