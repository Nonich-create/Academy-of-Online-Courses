using Students.DAL.Models;
using Students.MVC.ViewModels;
using AutoMapper;

namespace Students.MVC.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, EditStudentViewModel>()
           .ReverseMap();
            CreateMap<Manager, EditPersonViewModel>()
           .ReverseMap();
            CreateMap<Teacher, EditPersonViewModel>()
            .ReverseMap();
            CreateMap<ApplicationUser, UserViewModel>()
           .ReverseMap();
            CreateMap<Person, PersonViewModel>()
            .ReverseMap();
            CreateMap<Teacher, TeacherViewModel>()
            .ReverseMap();
            CreateMap<Student, StudentViewModel>()
            .ReverseMap();
            CreateMap<Student, DetaliStudentViewModel>()
            .ReverseMap();
            CreateMap<Manager, ManagerViewModel>()
            .ReverseMap();
            CreateMap<Assessment, AssessmentViewModel>()
            .ReverseMap();
            CreateMap<Lesson, LessonViewModel>()
            .ReverseMap();
            CreateMap<LessonTimes, LessonTimesViewModel>()
            .ReverseMap();
            CreateMap<Group, GroupViewModel>()
            .ReverseMap();
            CreateMap<Group, DetailGroupViewModel>()
            .ReverseMap();
            CreateMap<CourseApplication, CourseApplicationViewModel>()
            .ReverseMap();
            CreateMap<Course, CourseViewModel>()
            .ReverseMap();
            CreateMap<Course, DetalisCourseViewModel>()
            .ReverseMap();
        }
    }
}
