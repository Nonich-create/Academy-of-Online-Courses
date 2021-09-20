using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.DAL.Enum;
using Students.BLL.Mapper;

namespace Students.MVC.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<SelectListItem> GetEnumValueSelectList<TEnum>(this IHtmlHelper htmlHelper) where TEnum : struct
        {
            return new SelectList(Enum.GetValues(typeof(TEnum)).OfType<Enum>()
                .Select(x =>
                    new SelectListItem
                    {
                        Text = x.ToString(),
                        Value = x.ToString()
                    }), "Value", "Text");
        }

        public static GroupViewModel GroupToGroupViewModelMapping(this Group group, List<Manager> managers, List<Teacher> teachers, List<Course> courses)
        {
            GroupViewModel output = new GroupViewModel();

            output = Mapper.ConvertViewModel<GroupViewModel, Group>(group);
            output.Manageres = Mapper.ConvertListViewModel<ManagerViewModel, Manager>(managers);
            output.Teachers = Mapper.ConvertListViewModel<TeacherViewModel, Teacher>(teachers);
            output.Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>(courses);
            return output;
        }

        public static GroupViewModel GroupToGroupViewModelMapping(this Group group, Manager manager, Teacher teacher, Course course)
        {
            return GroupToGroupViewModelMapping(group, new List<Manager> { manager }, new List<Teacher> { teacher }, new List<Course> { course });
        }

        public static DetailGroupViewModel GroupToDetailGroupViewModelMapping(this Group group, Manager manager, Teacher teacher, Course course)
        {
            DetailGroupViewModel output = new DetailGroupViewModel();

            output = Mapper.ConvertViewModel<DetailGroupViewModel, Group>(group);
            output.Manager = Mapper.ConvertViewModel<ManagerViewModel, Manager>(manager);
            output.Teacher = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(teacher);
            output.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(course);

            return output;
        }

        //public static Group GroupViewModelToGroupMapping(this GroupViewModel group, List<ManagerViewModel> managers, List<TeacherViewModel> teachers, List<CourseViewModel> courses)
        //{
        //    Group output = new Group();

        //    output = Mapper.ConvertViewModel<Group, GroupViewModel>(group);
        //    output.Manageres = Mapper.ConvertListViewModel<Manager, ManagerViewModel>(managers);
        //    output.Teachers = Mapper.ConvertListViewModel<Teacher, TeacherViewModel>(teachers);
        //    output.Courses = Mapper.ConvertListViewModel<Course, CourseViewModel>(courses);
        //    return output;
        //}
    }

 
}
