using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Students.DAL.Models;
using System.Globalization;
using Students.BLL.DataAccess;
using Students.BLL.Services;
using Students.BLL.Options;

namespace Students.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("ru"),
                };
                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddDbContext<Context>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("Students.DAL")));
 
            services.AddIdentity<ApplicationUser, IdentityRole>()      
                .AddEntityFrameworkStores<Context>();

            services.AddScoped<ICourseApplicationService, CourseApplicationService>();
            services.AddScoped<IRepository<ApplicationUser>, ApplicationUserRepository>();
            services.AddScoped<IRepository<Student>, StudentRepository>();
            services.AddScoped<IRepository<Teacher>, TeacherRepository>();
            services.AddScoped<IRepository<Group>, GroupRepository>();
            services.AddScoped<IRepository<Manager>, ManagerRepository>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ILessonTimesService, LessonTimesService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UnitOfWork>();

            services.Configure<EmailAdminOptions>(Configuration.GetSection("EmailAdmin"));

            services.AddControllersWithViews();
        }
 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}