using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Students.DAL.Enum;
using Microsoft.Extensions.Logging;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using System.IO;
using System.Text;
using Students.BLL.GenerateFile;
using Microsoft.AspNetCore.Identity;

namespace Students.BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(UnitOfWork unitOfWork, ILogger<Student> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task PutRequest(string userId, int courseId)
        {
                var specCourseApplication = new CourseApplicationSearchWithItemsSpecifications(userId, courseId);
                if (await _unitOfWork.CourseApplicationRepository.GetAsync(specCourseApplication, false) != null)
                    throw new InvalidOperationException($"Вы уже подали заявку на этот курс");

                var specStudent = new StudentSearchWithItemsSpecifications(userId);
                var student = await _unitOfWork.StudentRepository.GetAsync(specStudent,true);
                if (student == null)
                    throw new InvalidOperationException($"Студента не существует");

                if (await _unitOfWork.CourseRepository.GetByIdAsync(courseId) == null)
                    throw new InvalidOperationException($"Курса {courseId} не существует"); 

                CourseApplication model = new()
                {
                    StudentId = student.Id,
                    CourseId = courseId,
                    ApplicationStatus = ApplicationStatus.Open
                };
            try
            {
                await _unitOfWork.CourseApplicationRepository.AddAsync(model);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Заявка принята Cтудент {student.Id}, Курс {courseId}");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка принятие заявки");
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

        public async Task<IEnumerable<Student>> GetAllAsync(int groupId)
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка студентов");
                var spec = new StudentWithItemsSpecifications((uint)groupId);
                return await _unitOfWork.StudentRepository.GetAsync(spec);
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
                var spec = new StudentWithItemsSpecifications((int)id);
                return await _unitOfWork.StudentRepository.GetAsync(spec, false);
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
                var spec = new StudentWithItemsSpecifications(id);
                return await _unitOfWork.StudentRepository.GetAsync(spec, false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение студента");
                return null;
            }
        }

        public async Task<Student> GetAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Получение студента");
                var spec = new StudentWithItemsSpecifications(userId);
                return await _unitOfWork.StudentRepository.GetAsync(spec, false);
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
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Студент создан");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,"Ошибка создания студента");
            }
        }

        public async Task CreateAsync(Student student, ApplicationUser user, string password)
        {
         
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "student");
                    student.UserId = user.Id;
                    await _unitOfWork.StudentRepository.AddAsync(student);
                    await _unitOfWork.SaveAsync();      
                    _logger.LogInformation("Студент создан");
                }
                else
                {
                    _logger.LogInformation("Ошибка создания пользователя");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания студента");
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
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(student.UserId);
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
                _logger.LogInformation($"Поиск студета {query}");
                return await _unitOfWork.StudentRepository.SearchAsync(query);
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Получение количество студентов");
            var specSearch = new StudentWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.StudentRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string query)
        {
            _logger.LogInformation($"Поиск студетов {query}");
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<Student>();
            var spec = new StudentWithItemsSpecifications(query);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск студетов {searchString}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Student>();
            var spec = new StudentWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Student>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение студентов");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка студентов");
                return Enumerable.Empty<Student>();
            }
            var spec = new StudentWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<Stream> GetContent(int studentId)
        {
            var specStudent = new StudentWithItemsSpecifications(studentId);
            var student = await _unitOfWork.StudentRepository.GetAsync(specStudent, true);

            var specCA = new CourseApplicationWithItemsSpecifications(studentId);
            var courseApplication = await _unitOfWork.CourseApplicationRepository.GetAsync(specCA);

            var sb = new StringBuilder();
            GenerateStream generateFile = new();
            sb.AppendLine($"ФИО: {student.Surname} {student.Name} {student.MiddleName}");
            sb.AppendLine($"Дата рождения {student.DateOfBirth}");
            sb.AppendLine($"Email {student.User.Email}");
            if (student.GroupId != null)
            {
                sb.AppendLine($"Курс {student.Group.Course.Name}");
                sb.AppendLine($"Номер группы {student.Group.NumberGroup}");
            }
            foreach (var item in courseApplication)
            {
                sb.AppendLine($"Номер заявки {item.Id} на курс {item.Course.Name};");
            }

            return generateFile.GenerateStreamFromString(sb.ToString());
        }
    }   
}
