using Students.BLL.DataAccess;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Students.DAL.Enum;
using System.Linq.Dynamic.Core;
using Students.BLL.Interface;
using Students.DAL.Specifications;

namespace Students.BLL.Services
{

    public class GroupService : IGroupService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public GroupService(UnitOfWork unitOfWork, ILogger<Group> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateAsync(Group item)
        {
            try
            {
                await _unitOfWork.GroupRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();  
                _logger.LogInformation("Группа создана");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка создания группы");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                Group group = await GetAsync(id);
                if (group != null)
                {
                    await _unitOfWork.LessonTimesRepository.DeleteRangeAsync(id);
                    await _unitOfWork.GroupRepository.DeleteAsync(group);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation(id, "Группа удалена");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка удаления группы");
            }
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Выполнения получения списка уроков");
                return await _unitOfWork.GroupRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получение списка групп");
                return Enumerable.Empty<Group>();
            }
        }

        public async Task<Group> GetAsync(int? groupId)
        {
            try
            {
                if (groupId == null)
                {
                    return null;
                }
                _logger.LogInformation("Получение группы");
                var spec = new GroupWithItemsSpecifications((uint)groupId);
                return (await _unitOfWork.GroupRepository.GetAsync(spec, false));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение группы");
                return null;
            }
        }

        public async Task<Group> GetAsync(int groupId)
        {
            try
            {
                _logger.LogInformation("Получение группы");
                var spec = new GroupWithItemsSpecifications((uint)groupId);
                return (await _unitOfWork.GroupRepository.GetAsync(spec,false));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получение группы");
                return null;
            }
        }

        public async Task StartGroup(int groupId)  
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                group.GroupStatus = GroupStatus.Training;
                var specStudents = new StudentWithItemsSpecifications((uint)group.Id);
                var students = await _unitOfWork.StudentRepository.GetAsync(specStudents);

                var specLesson = new LessonWithItemsSpecifications(group.CourseId);
                var lessones = await _unitOfWork.LessonRepository.GetAsync(specLesson);

                List<Assessment> assessments = new();
                List<LessonTimes> lessonTimes = new();

                FillAssements(group, students, lessones, assessments, lessonTimes);

                await _unitOfWork.AssessmentRepository.AddRangeAsync(assessments);
                await _unitOfWork.LessonTimesRepository.AddRangeAsync(lessonTimes);
                await _unitOfWork.GroupRepository.UpdateAsync(group);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Группа {group.Id} начала обучение");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка старта группы");
            }
        }

        private static void FillAssements(Group group, IEnumerable<Student> students, IEnumerable<Lesson> lessones, List<Assessment> assessments, List<LessonTimes> lessonTimes)
        {
            foreach (var itemLesson in lessones)
            {
                foreach (var itemStudent in students)
                {
                    assessments.Add(new() { LessonId = itemLesson.Id, StudentId = itemStudent.Id, Score = 0 });
                }
                lessonTimes.Add(new() { GroupId = group.Id, LessonId = itemLesson.Id });
            }
        }

        public async Task FinallyGroup(int groupId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                group.GroupStatus = GroupStatus.Close;

                var specStudents = new StudentWithItemsSpecifications((uint)group.Id);
                var students = await _unitOfWork.StudentRepository.GetAsync(specStudents);

                foreach (var item in students)
                {
                    item.GroupId = null;
                }

                var specLessonTimes = new LessonTimesWithItemsSpecifications((uint)group.Id);
                var lessonTimes = await _unitOfWork.LessonTimesRepository.GetAsync(specLessonTimes);

                await _unitOfWork.LessonTimesRepository.DeleteRangeAsync(lessonTimes);
                await _unitOfWork.StudentRepository.UpdateRangeAsync(students);
                await _unitOfWork.GroupRepository.UpdateAsync(group);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Группа {group.Id} окончила обучение");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка завершение группы");
            }
        }

        public async Task CancelGroup(int groupId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                group.GroupStatus = GroupStatus.Cancelled;

                var specStudents = new StudentWithItemsSpecifications((uint)group.Id);
                var students = await _unitOfWork.StudentRepository.GetAsync(specStudents);

                List<CourseApplication> courseApplications = new();

                foreach (var item in students)
                {
                    var specCourseApplication = new CourseApplicationWithItemsSpecifications((uint)item.Id ,(uint)group.CourseId);
                    var courseApplication = await _unitOfWork.CourseApplicationRepository.GetAsync(specCourseApplication,true);
                    courseApplication.ApplicationStatus = ApplicationStatus.Cancelled;
                    courseApplication.UpdateDate = DateTime.Now;
                    courseApplications.Add(courseApplication);
                    item.GroupId = null;
                }

                await _unitOfWork.CourseApplicationRepository.UpdateRangeAsync(courseApplications);
                await _unitOfWork.StudentRepository.UpdateRangeAsync(students);
                await _unitOfWork.GroupRepository.UpdateAsync(group);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Группа {group.Id} отмененна");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка отмены группы");
            }
        }

         
        public async Task RefreshGroup(int groupId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                group.GroupStatus = GroupStatus.Set;

                await _unitOfWork.GroupRepository.UpdateAsync(group);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Группа {group.Id} обновлена");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка обновление группы");
            }
        }

        public async Task<Group> Update(Group item)
        {
            try
            {
                await _unitOfWork.GroupRepository.UpdateAsync(item);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Группа изменена");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка редактирования группы");
                return item;
            }
        }

        public async Task<Group> SearchAsync(string query)
        {
            try
            {
                _logger.LogInformation("Поиск группы");
                return await _unitOfWork.GroupRepository.SearchAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка поиска группы");
                return null;
            }
        }

        public async Task<int> GetCount(int teacherId)
        {
            _logger.LogInformation($"Получение количество групп преподователя {teacherId}");
            var spec = new GroupWithItemsSpecifications(teacherId);
            return (await _unitOfWork.GroupRepository.CountAsync(spec));
        }

        public async Task<int> GetCount(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation("Получение количество групп");
            var specSearch = new GroupWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.GroupRepository.CountAsync(specSearch);
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string searchString, EnumSearchParameters searchParametr)
        {
            _logger.LogInformation($"Поиск группы {searchString}");
            if (string.IsNullOrEmpty(searchString) || searchParametr == EnumSearchParameters.None)
                return Enumerable.Empty<Group>();
            var spec = new GroupWithItemsSpecifications(searchString, searchParametr);
            return await _unitOfWork.GroupRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Group>> SearchAllAsync(string query)
        {
            _logger.LogInformation($"Поиск группы {query}");
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<Group>();
            var spec = new GroupWithItemsSpecifications(query);
            return await _unitOfWork.GroupRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Group>> IndexView(string searchString, EnumSearchParameters searchParametr, int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение групп");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка получение групп");
                return Enumerable.Empty<Group>();
            }
            var spec = new GroupWithItemsSpecifications(currentPage, pageSize, searchString, searchParametr);
            return await _unitOfWork.GroupRepository.GetAsync(spec);
        }

        public async Task<IEnumerable<Group>> IndexView(int teacherId,int currentPage, int pageSize = 10)
        {
            _logger.LogInformation("Получение групп");
            if (currentPage <= 0 || pageSize <= 0)
            {
                _logger.LogInformation("Ошибка получение групп");
                return Enumerable.Empty<Group>();
            }
            var spec = new GroupWithItemsSpecifications(teacherId,currentPage, pageSize);
            return await _unitOfWork.GroupRepository.GetAsync(spec);
        }
    }

}
