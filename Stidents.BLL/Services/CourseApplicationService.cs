using Microsoft.Extensions.Logging;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using Students.BLL.Interface;

namespace Students.BLL.Services
{
    public class CourseApplicationService : ICourseApplicationService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CourseApplicationService(UnitOfWork unitOfWork, ILogger<CourseApplication> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Cancel(CourseApplication model)
        { 
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(model.StudentId);
            var group =  (await _unitOfWork.GroupRepository.GetAllAsync()).FirstOrDefault(g => g.CourseId == model.CourseId
            && g.Id == (int)student.GroupId 
            && g.GroupStatus == EnumGroupStatus.Training);
            if (group == null)
            {
                student.GroupId = null;
                await _unitOfWork.StudentRepository.UpdateAsync(student);
                model.ApplicationStatus = EnumApplicationStatus.Cancelled;
                await _unitOfWork.CourseApplicationRepository.UpdateAsync(model);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Заявка студента {student.Id} на курс {model.CourseId} отменена");
            }
            else 
            {
                _logger.LogInformation($"На данный момент студент {student.Id} обучается в группе {group.Id}");
                throw new InvalidOperationException($"На данный момент студент {student.Id} обучается в группе {group.Id}");
            }
        }

        public async Task CreateAsync(CourseApplication item)
        {
            try
            {
                await _unitOfWork.CourseApplicationRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Заявка создана");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания заяки");
            }
        }


        public async Task DeleteAsync(int id)
        {
            try
            {
                CourseApplication courseApplication = await _unitOfWork.CourseApplicationRepository.GetByIdAsync(id);
                if (courseApplication != null)
                {
                    await _unitOfWork.CourseApplicationRepository.DeleteAsync(courseApplication);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Заяка удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заяки");
            }
        }

        public async Task DeleteAsyncAll(int id)
        {
            try
            {   
                await _unitOfWork.CourseApplicationRepository.DeleteAsyncAllByStudentId(id);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(id, "Заяки студента удалены"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заявок");
            }
        }

        public async Task Enroll(CourseApplication model) 
        {
            try
            {
                var students = (await _unitOfWork.StudentRepository.GetAllAsync()).Where(s => s.GroupId != null);
                var group = (await _unitOfWork.GroupRepository.GetAllAsync()).First(g => g.CourseId == model.CourseId &&
               g.GroupStatus == EnumGroupStatus.Set &&
               g.CountMax > students.Count(s => s.GroupId == g.Id));
                if (group == null) { throw new InvalidOperationException($"На данный момент подходящих групп нет"); }
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(model.StudentId);
                if (student.GroupId != null) { throw new InvalidOperationException($"{student.Surname} {student.Name} {student.MiddleName} уже находится в группе"); }
                student.GroupId = group.Id;
                await _unitOfWork.StudentRepository.UpdateAsync(student);
                model.ApplicationStatus = EnumApplicationStatus.Close;
                await _unitOfWork.CourseApplicationRepository.UpdateAsync(model);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Студент {model.StudentId} зачислен в группу {group.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка обработке заявки");
            }
        }

        public async Task<IEnumerable<CourseApplication>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка заявок");
                return await _unitOfWork.CourseApplicationRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка заявок");
                return Enumerable.Empty<CourseApplication>();
            }
        }


        public async Task<CourseApplication> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение заяки");
                return await _unitOfWork.CourseApplicationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение заяки");
                return null;
            }
        }
       
        public async Task<CourseApplication> Update(CourseApplication item)
        {
            try
            {
                await _unitOfWork.CourseApplicationRepository.UpdateAsync(item);
                _logger.LogInformation("Заявка изменена");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования заяки");
                return item;
            }
        }

        public async Task<CourseApplication> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск заяки");
                return await _unitOfWork.CourseApplicationRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска заяки");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.CourseApplicationRepository.GetCourseApplicationListAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<CourseApplication>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.CourseApplicationRepository.GetCourseApplicationListAsync())
                .OrderBy(c => c.Course.Name).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<CourseApplication>();
            return (await _unitOfWork.CourseApplicationRepository.GetCourseApplicationListAsync()).AsQueryable()
                .Where(query);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<CourseApplication>();
            return (await _unitOfWork.CourseApplicationRepository.GetCourseApplicationListAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<CourseApplication>();
            return (await _unitOfWork.CourseApplicationRepository.GetCourseApplicationListAsync()).AsQueryable()
                .OrderBy(c => c.Course.Name)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<CourseApplication>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }
}
