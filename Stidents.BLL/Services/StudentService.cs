using System.Threading.Tasks;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Students.DAL.Enum;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Students.BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;

        public StudentService(UnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger<Student> logger)
        {
            _unitOfWork = unitOfWork;
            cache = memoryCache;
            _logger = logger;
        }

        public async Task Save()
        {
            await _unitOfWork.Save();
        }

        public async Task PutRequest(int StudentId, int СourseId)
        {
            try
            {
                if (_unitOfWork.ApplicationCourseRepository.GetAllAsync().Result.Any(a => a.CourseId == СourseId && a.StudentId == StudentId) == true)
                {
                    throw new InvalidOperationException($"Вы уже подали заявку на этот курс");
                }
                var student = await _unitOfWork.StudentRepository.GetAsync(StudentId);
                if (student == null) { throw new InvalidOperationException($"Такого пользователя не существует"); }
                var course = await _unitOfWork.CourseRepository.GetAsync(СourseId);
                if (course == null) { throw new InvalidOperationException($"Такого курса не существует"); }
                CourseApplication model = new()
                {
                    StudentId = StudentId,
                    CourseId = СourseId,
                    ApplicationStatus = EnumApplicationStatus.Открыта
                };
                await _unitOfWork.ApplicationCourseRepository.CreateAsync(model);
                await _unitOfWork.Save();
                _logger.LogInformation($"Заявка принята Id Cтудента {StudentId}, Id Курса {СourseId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка принятия заявки");
            }
        }

        public async Task<List<Student>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка студентов");
                return await _unitOfWork.StudentRepository.GetAllAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка студентов");
                return null;
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
                if (!cache.TryGetValue(id, out Student student))
                {
                    _logger.LogInformation("Кэша нету");
                    student = await _unitOfWork.StudentRepository.GetAsync(id);
                    if (student != null)
                    {
                        cache.Set(student.Id, student,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return student;
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
                if (!cache.TryGetValue(id, out Student student))
                {
                    _logger.LogInformation("Кэша нету");
                    student = await _unitOfWork.StudentRepository.GetAsync(id);
                    if (student != null)
                    {
                        cache.Set(student.Id, student,
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                else
                {
                    _logger.LogInformation("Кэш есть");
                }
                return student;
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
                await _unitOfWork.StudentRepository.CreateAsync(item);
                _logger.LogInformation("Студент создан");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Добавлен в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
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
                var student =  await _unitOfWork.StudentRepository.Update(item);
                _logger.LogInformation("Студент изменен");
                int n = await _unitOfWork.Save();
                if (n > 0)
                {
                    _logger.LogInformation("Студент добавлен в кэш");
                    cache.Set(item.Id, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                }
                return student;
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
                await _unitOfWork.StudentRepository.DeleteAsync(id);
                _logger.LogInformation(id,"Студент удален"); ;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления студента");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.StudentRepository.ExistsAsync(id);
        }
    }
}
