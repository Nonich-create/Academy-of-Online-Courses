using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Students.DAL.Enum;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Students.BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public StudentService(UnitOfWork unitOfWork, ILogger<Student> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task PutRequest(int StudentId, int СourseId)
        {
            try
            {
                if ((await _unitOfWork.CourseApplicationRepository.GetAllAsync()).Any(a => a.CourseId == СourseId && a.StudentId == StudentId))
                {
                    throw new InvalidOperationException($"Вы уже подали заявку на этот курс");
                }
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(StudentId);
                if (student == null) { throw new InvalidOperationException($"Такого пользователя не существует"); }
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(СourseId);
                if (course == null) { throw new InvalidOperationException($"Такого курса не существует"); }
                CourseApplication model = new()
                {
                    StudentId = StudentId,
                    CourseId = СourseId,
                    ApplicationStatus = EnumApplicationStatus.Open
                };
                await _unitOfWork.CourseApplicationRepository.AddAsync(model);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Заявка принята Id Cтудента {StudentId}, Id Курса {СourseId}");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка принятия заявки");
            }
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка студентов");
                return await _unitOfWork.StudentRepository.GetAllAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка студентов");
                return Enumerable.Empty<Student>();
            }
        }

        public async Task<Student> GetAsync(int? id)
        {
            try
            {
                _logger.LogInformation("Получение студента");
                if (id == null)
                {
                    return null;
                }
                return await _unitOfWork.StudentRepository.GetByIdAsync((int)id); 
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex,"Ошибка при получение студента");
                return null;
            }
        }

        public async Task<Student> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Получение студента");
                return await _unitOfWork.StudentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение студента");
                return null;
            }
        }

        public async Task CreateAsync(Student item)
        {
            try
            {
                await _unitOfWork.StudentRepository.AddAsync(item);
                _logger.LogInformation("Студент создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"Ошибка создания студента");
            }
        }

        public async Task<Student> Update(Student item)
        {
            try
            {
                await _unitOfWork.StudentRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Студент изменен");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования студента");
                return item;
            }

        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Student student = await _unitOfWork.StudentRepository.GetByIdAsync(id);
                ApplicationUser applicationUser = await _unitOfWork.ApplicationUsersRepository.GetByIdAsync(student.UserId);
                if (student != null)
                {
                    await _unitOfWork.CourseApplicationRepository.DeleteAsyncAllByStudentId(id);
                    await _unitOfWork.ApplicationUsersRepository.DeleteAsync(applicationUser);
                    await _unitOfWork.StudentRepository.DeleteAsync(student);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Студент удален"); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления студента");
            }
        }

        public async Task<Student> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск студета");
                return await _unitOfWork.StudentRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска студента");
                return null;
            }
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
            {
                return (await _unitOfWork.StudentRepository.GetStudentListAsync()).Count();
            }
            return (await SearchAllAsync(searchString, searchParametr)).Count();
        }

        public async Task<IEnumerable<Student>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            return (await _unitOfWork.StudentRepository.GetStudentListAsync())
                .OrderBy(s => s.Surname).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Student>();
            return (await _unitOfWork.StudentRepository.GetStudentListAsync()).AsQueryable()
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Student>();
            return (await _unitOfWork.StudentRepository.GetStudentListAsync()).AsQueryable()
                .OrderBy(s => s.Surname)
                .Where($"{searchParametr.ToString().Replace('_', '.')}.Contains(@0)", searchString)
                .Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Student>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(searchString, searchParametr, currentPage, pageSize);
            }
            return await GetPaginatedResult(currentPage, pageSize);
        }
    }
}
