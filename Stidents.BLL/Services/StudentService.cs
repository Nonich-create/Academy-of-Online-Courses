using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Students.DAL.Enum;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using Students.BLL.Interface;
using Students.DAL.Specifications;
using System.IO;
using System.Text;
using Students.BLL.GenerateFile;

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
                    ApplicationStatus = ApplicationStatus.Open
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
                var spec = new StudentWithItemsSpecifications();
                return (await _unitOfWork.StudentRepository.CountAsync(spec));
            }
            var specSearch = new StudentWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.StudentRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<Student>();
            var spec = new StudentWithItemsSpecifications(query);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Student>();
            var spec = new StudentWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Student>> SearchAllAsync(int currentPage, int pageSize,string searchString, EnumSearchParameters searchParametr)
        {
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Student>();
            var spec = new StudentWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }


        public async Task<IEnumerable<Student>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            if (!String.IsNullOrEmpty(searchString) && searchParametr != EnumSearchParameters.None)
            {
                return await SearchAllAsync(currentPage,pageSize,searchString,searchParametr);
            }
            var spec = new StudentWithItemsSpecifications(currentPage, pageSize);
            return await _unitOfWork.StudentRepository.GetAsync(spec);
        }

        public async Task<Stream> GetContent(int studentId)
        {
            var spec = new StudentWithItemsSpecifications(studentId);
            var specCA = new CourseApplicationWithItemsSpecifications(studentId);
            var student = await _unitOfWork.StudentRepository.GetAsync(spec,true);
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
