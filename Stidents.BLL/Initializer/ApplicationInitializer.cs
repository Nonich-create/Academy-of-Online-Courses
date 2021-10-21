using Microsoft.AspNetCore.Identity;
using Students.DAL.Enum;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Students.BLL.Options;
using System.Linq;
using System.Collections.Generic;
using System;
using Students.BLL.Initializer;

namespace Students.DAL.Models
{
    public static class ApplicationInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<EmailAdminOptions> emailAdminOptions, Context context)
        {
            EmailAdminOptions emailAdmin = emailAdminOptions.Value;
            GenerateTestData GenerateTestdata = new();
            Random rnd = new();
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("student") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("student"));
            }
            if (await roleManager.FindByNameAsync("teacher") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("teacher"));
            }
            if (await roleManager.FindByNameAsync("manager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("manager"));
            }
            if (await userManager.FindByNameAsync(emailAdmin.Email) == null)
            {
                ApplicationUser admin = new() { Email = emailAdmin.Email, UserName = emailAdmin.Email };
                IdentityResult result = await userManager.CreateAsync(admin, emailAdmin.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
            if (!context.Manager.Any())
            {
                List<string> Email = new();
                List<string> Phone = new();
                Email.AddRange(GenerateTestData.EmailListGenerate(3));
                Phone.AddRange(GenerateTestData.PhoneListGenerate(3));
                for (int i = 0; i < 3; i++)
                {
                    ApplicationUser user = new() { Email = Email[i], UserName = Email[i], PhoneNumber = Phone[i] };
                    IdentityResult result = await userManager.CreateAsync(user, "C3F-r8n-z6t-Nui");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "manager");
                        Manager manager = new()
                        {
                            Name = GenerateTestdata.Name[rnd.Next(0, GenerateTestdata.Name.Length)],
                            Surname = GenerateTestdata.famile[rnd.Next(0, GenerateTestdata.famile.Length)],
                            MiddleName = "Анатолевич",
                            UserId = user.Id,
                            URLImagePhoto = "https://scuolamediavaldocco.it/wp-content/uploads/2019/07/Progetto-senza-titolo.png"
                        };
                        context.Manager.Add(manager);
                    }
                }
                context.SaveChanges();
            }
            if (!context.Teachers.Any())
            {
                List<string> Email = new();
                List<string> Phone = new();
                Email.AddRange(GenerateTestData.EmailListGenerate(3));
                Phone.AddRange(GenerateTestData.PhoneListGenerate(3));
                for (int i = 0; i < 3; i++)
                {
                    ApplicationUser user = new() { Email = Email[i], UserName = Email[i], PhoneNumber = Phone[i] };
                    IdentityResult result = await userManager.CreateAsync(user, "C3F-r8n-z6t-Nui");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "teacher");
                        Teacher teacher = new() 
                        {
                            Name = GenerateTestdata.Name[rnd.Next(0, GenerateTestdata.Name.Length)],
                            Surname = GenerateTestdata.famile[rnd.Next(0, GenerateTestdata.famile.Length)],
                            MiddleName = "Александрович",
                            UserId = user.Id,
                            URLImagePhoto = "https://scuolamediavaldocco.it/wp-content/uploads/2019/07/Progetto-senza-titolo.png"
                        };
                        context.Teachers.Add(teacher);
                    }
                }
                context.SaveChanges();
            }
            if (!context.Courses.Any())
            {
                await context.Courses.AddRangeAsync(new List<Course>
                 {
                     new Course
                     {
                         Name = "Программирование на C# для начинающих",
                         Description = "Как эффективнее работать с C#. Для эффективной работы полезно знать о дополнительных инструментах. Вот некоторые из них:" +
                         "1. WPF (Windows Presentation Foundation) поможет легко и быстро создать интерфейс для любых экранов, уменьшает количество кода, отображающего интерфейс." +
                         "2. Xamarin — фреймворк для разработки кроссплатформенных приложений для Windows Phone, Android и iOS. То есть вы напишете один код, и он сразу будет работать на всех основных платформах." +
                         "3. ASP.NET помогает просто и быстро связать серверный код с клиентским в серверных приложениях и веб-разработке." +
                         "4. Entity Framework — один из лучших фреймворков для работы с базами данных. Позволяет работать с данными как с объектами." +
                         "5. LINQ (Language Integrated Query) — мини-язык запросов, встроенный в C# и позволяющий легко выбирать, фильтровать, сортировать и группировать любые данные из любых источников: баз данных, массивов, структур, файлов и так далее." +
                         "6. Visual Studio — среда разработки, созданная специально для языка C#.",
                         Price = (decimal)2300.99,
                         Visible = true,
                         URLImagePhoto = "https://i.pinimg.com/originals/07/b6/1f/07b61f1a6ba560e352178dfbf6432ad9.png",
                         Duration = "51 дней"

                     },
                     new Course
                     {
                         Name = "Программирование на C++ для начинающих",
                         Description = "Здесь представлены более 25 уроков, где с нуля рассматриваются основы и тонкости" +
                         "языка С++ и программирования в целом. Есть пошаговые создания игр на С++ с помощью библиотек" +
                         "MFC и SFML, и более 70 практических заданий для проверки ваших навыков программирования. ",
                         Price = (decimal)860.80,
                         Visible = true,
                         URLImagePhoto = "https://akonit.net/image/cache/catalog/feed_3/35003-348084-1000x1000.jpg",
                         Duration = "90 дней"
                     },
                     new Course
                     {
                         Name = "1C-Специалист",
                         Description = "Здесь представлены более 25 уроков, где с нуля рассматриваются основы и тонкости 1С",
                         Price = (decimal)375.80,
                         Visible = true,
                         URLImagePhoto = "https://media2.24aul.ru/imgs/52536dc9787ad725b8a95d7a/kursy-1s-v-krasnoyarske-kursy-bukhgaltera-kursy-dopolnitelnogo-1-3260476.jpg",
                         Duration = "60 дней"
                     },
                     new Course
                     {
                         Name = "Английский язык  для начинающих",
                         Description = "Здесь представлены более 25 уроков, где с нуля рассматриваются основы и тонкости" +
                         "языка",
                         Price = (decimal)359.80,
                         Visible = true,
                         URLImagePhoto = "https://miit-ief.ru/wp-content/uploads/2017/09/english2.jpg",
                         Duration = "50 дней"
                     },
                     new Course
                     {
                         Name = "Факультет Java-разработки",
                         Description = "Здесь представлены более 200 уроков, где с нуля рассматриваются основы и тонкости" +
                         "языка Java и программирования в целом.",
                         Price = (decimal)2300.80,
                         Visible = true,
                         URLImagePhoto = "https://www.greatsolutionspinjore.com/img/course0006.jpg",
                         Duration = "360 дней"
                     }
                 });
                context.SaveChanges();
            }

            if (!context.Lessons.Any())
            {
                List<Lesson> lessons = new();
                for (int i = 1; i < 51; i++)
                {
                    lessons.Add(
                    new Lesson
                    {
                        Name = $"Урок №{i} C#",
                        NumberLesson = i,
                        Homework = $"Домашние задание №{i}",
                        Description = $"Тема урока №{i}",
                        CourseId = 2,               
                    });
                }
                for (int i = 1; i < 91; i++)
                {
                    lessons.Add(
                    new Lesson
                    {
                        Name = $"Урок №{i} C++",
                        NumberLesson = i,
                        Homework = $"Домашние задание №{i}",
                        Description = $"Тема урока №{i}",
                        CourseId = 1,
                    });
                }
                for (int i = 1 ; i < 61; i++)
                {
                    lessons.Add(
                    new Lesson
                    {
                        Name = $"Урок №{i} 1C",
                        NumberLesson = i,
                        Homework = $"Домашние задание №{i}",
                        Description = $"Тема урока №{i}",
                        CourseId = 3,
                    });
                }
                for (int i = 1; i < 51; i++)
                {
                    lessons.Add(
                    new Lesson
                    {
                        Name = $"Урок №{i} English",
                        NumberLesson = i,
                        Homework = $"Домашние задание №{i}",
                        Description = $"Тема урока №{i}",
                        CourseId = 4,
                    });
                }
                for (int i = 1; i < 361; i++)
                {
                    lessons.Add(
                    new Lesson
                    {
                        Name = $"Урок №{i} Java",
                        NumberLesson = i,
                        Homework = $"Домашние задание №{i}",
                        Description = $"Тема урока №{i}",
                        CourseId = 5,
                    });
                }
                await context.Lessons.AddRangeAsync(lessons);
                context.SaveChanges();
            }
            if (!context.Groups.Any())
            {
                await context.Groups.AddRangeAsync(new List<Group>
                {
                    new Group
                    {
                        CourseId = 2,
                        ManagerId = 1,
                        TeacherId = 1,
                        NumberGroup = "С# 101",
                        DateStart = new DateTime(2022, 01, 07),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 25
                    },
                    new Group
                    {
                        CourseId = 2,   
                        ManagerId = 1,
                        TeacherId = 1,
                        NumberGroup = "С# 201",
                        DateStart = new DateTime(2022, 01, 07),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 25
                    },
                    new Group
                    {
                        CourseId = 1,
                        ManagerId = 2,
                        TeacherId = 2,
                        NumberGroup = "С++ 101",
                        DateStart = new DateTime(2022, 01, 07),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 20,
                    },
                    new Group
                    {
                        CourseId = 3,
                        ManagerId = 2,
                        TeacherId = 2,
                        NumberGroup = "1C 101",
                        DateStart = new DateTime(2021, 11, 01),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 15
                    },
                    new Group
                    {
                        CourseId = 4,
                        ManagerId = 3,
                        TeacherId = 3,
                        NumberGroup = "English 101",
                        DateStart = new DateTime(2021, 11, 01),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 40
                    },
                        new Group
                    {
                        CourseId = 5,
                        ManagerId =3,
                        TeacherId = 3,
                        NumberGroup = "Java 101",
                        DateStart = new DateTime(2020, 10, 01),
                        GroupStatus = GroupStatus.Set,
                        CountMax = 30
                    }

                });
                context.SaveChanges();
            }
            if (!context.Students.Any())
            {
                List<string> Email = new();
                List<string> Phone = new();
                Email.AddRange(GenerateTestData.EmailListGenerate(250));
                Phone.AddRange(GenerateTestData.PhoneListGenerate(250));
                for (int i = 0; i < 250; i++)
                {
                    ApplicationUser user = new() { Email = Email[i], UserName = Email[i], PhoneNumber = Phone[i] };
                    IdentityResult result = await userManager.CreateAsync(user, "C3F-r8n-z6t-Nui");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "student");
                        Student student = new()
                        {
                            Name = GenerateTestdata.Name[rnd.Next(0, GenerateTestdata.Name.Length)],
                            Surname = GenerateTestdata.famile[rnd.Next(0, GenerateTestdata.famile.Length)],
                            MiddleName = "Алексеевич",
                            UserId = user.Id,
                            URLImagePhoto = "https://scuolamediavaldocco.it/wp-content/uploads/2019/07/Progetto-senza-titolo.png",
                            DateOfBirth = GenerateTestData.DateGenerate()
                        };
                        context.Students.Add(student);
                    }
                }
                context.SaveChanges();
            }
            if (!context.CourseApplication.Any())
            {
                for (int i = 1; i < 30; i++)
                {
                    CourseApplication courseApplication = new()
                    {
                        ApplicationStatus = ApplicationStatus.Open,
                        CourseId = 1,
                        StudentId = i                       
                    };
                    context.CourseApplication.Add(courseApplication);
                }
                for (int i = 30; i < 45; i++)
                {
                    CourseApplication courseApplication = new()
                    {
                        ApplicationStatus = ApplicationStatus.Open,
                        CourseId = 2,
                        StudentId = i
                    };
                    context.CourseApplication.Add(courseApplication);
                }
                for (int i = 45; i < 77; i++)
                {
                    CourseApplication courseApplication = new()
                    {
                        ApplicationStatus = ApplicationStatus.Open,
                        CourseId = 3,
                        StudentId = i
                    };
                    context.CourseApplication.Add(courseApplication);
                }
                for (int i = 77; i < 100; i++)
                {
                    CourseApplication courseApplication = new()
                    {
                        ApplicationStatus = ApplicationStatus.Open,
                        CourseId = 4,
                        StudentId = i
                    };
                    context.CourseApplication.Add(courseApplication);
                }
                for (int i = 100; i < 150; i++)
                {
                    CourseApplication courseApplication = new()
                    {
                        ApplicationStatus = ApplicationStatus.Open,
                        CourseId = 5,
                        StudentId = i
                    };
                    context.CourseApplication.Add(courseApplication);
                }
                context.SaveChanges();
            }
        }
    }
}

