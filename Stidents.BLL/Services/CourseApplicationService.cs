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
using Students.DAL.Specifications;

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

        public async Task Cancel(int courseApplicationId)
        {
            var courseApplication = await _unitOfWork.CourseApplicationRepository.GetByIdAsync(courseApplicationId);
            if (courseApplication == null)
            {
                _logger.LogInformation($"Заявка студента {courseApplicationId} не существует");
                return;
            }
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(courseApplication.StudentId);
            var group =  (await _unitOfWork.GroupRepository.GetAllAsync()).FirstOrDefault(g => g.CourseId == courseApplication.CourseId
            && g.Id == (int)student.GroupId 
            && g.GroupStatus == EnumGroupStatus.Training);
            if (group == null)
            {
                student.GroupId = null;
                await _unitOfWork.StudentRepository.UpdateAsync(student);
                courseApplication.ApplicationStatus = EnumApplicationStatus.Cancelled;
                await _unitOfWork.CourseApplicationRepository.UpdateAsync(courseApplication);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Заявка студента {student.Id} на курс {courseApplication.CourseId} отменена");
            }
            else 
            {
                _logger.LogInformation($"На данный момент студент {student.Id} обучается в группе {group.Id}");
                throw new InvalidOperationException($"На данный момент студент {student.Id} обучается в группе {group.Id}");
            }
        }

        public async Task CancelApplication(int courseApplicationId)
        {
            try
            {
                var courseApplication = await _unitOfWork.CourseApplicationRepository.GetByIdAsync(courseApplicationId);
                if (courseApplication == null)
                {
                    _logger.LogInformation($"Заявка студента {courseApplicationId} не существует");
                    return;
                }
                if (courseApplication.ApplicationStatus == EnumApplicationStatus.Open)
                {
                    courseApplication.ApplicationStatus = EnumApplicationStatus.Cancelled;
                    await _unitOfWork.SaveAsync();
                }
                _logger.LogInformation($"Заявка студента {courseApplication.StudentId} на курс {courseApplication.CourseId} отменена");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Ошибка обновления заявки {courseApplicationId}");
            }
        }
      
        public async Task Open(int courseApplicationId)
        {
            try
            { 
            var courseApplication = await _unitOfWork.CourseApplicationRepository.GetByIdAsync(courseApplicationId);
            if (courseApplication == null)
            {
                _logger.LogInformation($"Заявка студента {courseApplicationId} не существует");
                return;
            }
            courseApplication.ApplicationStatus = EnumApplicationStatus.Open;
            await _unitOfWork.CourseApplicationRepository.UpdateAsync(courseApplication);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation($"Заявка студента {courseApplication.StudentId} на курс {courseApplication.CourseId} Обновлена");
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, $"Ошибка обновления заявки {courseApplicationId}");
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
                _logger.LogInformation(ex, "Ошибка создания заявки");
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
                    _logger.LogInformation(id, "Заявка удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заявки");
            }
        }

        public async Task DeleteAsyncAll(int id)
        {
            try
            {   
                await _unitOfWork.CourseApplicationRepository.DeleteAsyncAllByStudentId(id);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation(id, "Заявки студента удалены"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления заявок");
            }
        }

        public async Task Enroll(int courseApplicationId) 
        {
            try
            {
                var courseApplication = await _unitOfWork.CourseApplicationRepository.GetByIdAsync(courseApplicationId);
                if (courseApplication == null)
                {
                    _logger.LogInformation($"Заявка студента {courseApplicationId} не существует");
                    return;
                }
                var students = (await _unitOfWork.StudentRepository.GetAllAsync()).Where(s => s.GroupId != null);
                var group = (await _unitOfWork.GroupRepository.GetAllAsync()).First(g => g.CourseId == courseApplication.CourseId &&
               g.GroupStatus == EnumGroupStatus.Set &&
               g.CountMax > students.Count(s => s.GroupId == g.Id));
                if (group == null) { throw new InvalidOperationException($"На данный момент подходящих групп нет"); }
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(courseApplication.StudentId);
                if (student.GroupId != null) { throw new InvalidOperationException($"{student.Surname} {student.Name} {student.MiddleName} уже находится в группе"); }
                student.GroupId = group.Id;
                await _unitOfWork.StudentRepository.UpdateAsync(student);
                courseApplication.ApplicationStatus = EnumApplicationStatus.Close;
                await _unitOfWork.CourseApplicationRepository.UpdateAsync(courseApplication);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Студент {courseApplication.StudentId} зачислен в группу {group.Id}");
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
                _logger.LogInformation("Получение заявки");
                return await _unitOfWork.CourseApplicationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение заявки");
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
                _logger.LogInformation(ex, "Ошибка редактирования заявки");
                return item;
            }
        }

        public async Task<CourseApplication> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск заявки");
                return await _unitOfWork.CourseApplicationRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска заявки");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                var spec = new CourseApplicationWithItemsSpecifications();
                return (await _unitOfWork.CourseApplicationRepository.CountAsync(spec));
            }
            var specSearch = new CourseApplicationWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.CourseApplicationRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<CourseApplication>();
            var spec = new CourseApplicationWithItemsSpecifications(query);
            return await _unitOfWork.CourseApplicationRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<CourseApplication>();
            var spec = new CourseApplicationWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.CourseApplicationRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<CourseApplication>> SearchAllAsync(int currentPage, int pageSize, string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<CourseApplication>();
            var spec = new CourseApplicationWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.CourseApplicationRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<CourseApplication>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage, pageSize, searchString, searchParametr);
            }
            var spec = new CourseApplicationWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.CourseApplicationRepository.GetAsync(spec);
        }
    }
}
