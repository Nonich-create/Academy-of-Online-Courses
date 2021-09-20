using Students.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Students.BLL.DataAccess
{
    public class UnitOfWork : IDisposable
    {
        private readonly Context _db;
        private ApplicationUserRepository _applicationUserRepository;
        private StudentRepository _studentRepository;
        private TeacherRepository _teacherRepository;
        private GroupRepository _groupRepository;
        private ManagerRepository _managerRepository;
        private LessonRepository _lessonRepository;
        private CourseRepository _courseRepository;
        private AssessmentRepository _assessmentRepository;
        private CourseApplicationRepository _courseApplicationRepository;
        private LessonTimesRepository _lessonTimesRepository;

        private bool disposed = false;

        public UnitOfWork(Context db)
        {
            _db = db;
         
        }

        public CourseApplicationRepository CourseApplicationRepository
        {
            get
            {
                if (_courseApplicationRepository == null)
                    _courseApplicationRepository = new CourseApplicationRepository(_db);
                return _courseApplicationRepository;
            }
        }

        public ManagerRepository ManagerRepository
        {
            get
            {
                if (_managerRepository == null)
                    _managerRepository = new ManagerRepository(_db);
                return _managerRepository;
            }
        }

        public GroupRepository GroupRepository
        {
            get
            {
                if (_groupRepository == null)
                    _groupRepository = new GroupRepository(_db);
                return _groupRepository;
            }
        }

        public ApplicationUserRepository ApplicationUsers
        {
            get
            {
                if (_applicationUserRepository == null)
                    _applicationUserRepository = new ApplicationUserRepository(_db);
                return _applicationUserRepository;
            }
        }

        public StudentRepository StudentRepository
        {
            get
            {
                if (_studentRepository == null)
                    _studentRepository = new StudentRepository(_db);
                return _studentRepository;
            }
        }

        public TeacherRepository TeacherRepository
        {
            get
            {
                if (_teacherRepository == null)
                    _teacherRepository = new TeacherRepository(_db);
                return _teacherRepository;
            }
        }
        public LessonRepository LessonRepository
        {
            get
            {
                if (_lessonRepository == null)
                    _lessonRepository = new LessonRepository(_db);
                return _lessonRepository;
            }
        }
        public CourseRepository CourseRepository
        {
            get
            {
                if (_courseRepository == null)
                    _courseRepository = new CourseRepository(_db);
                return _courseRepository;
            }
        }
        public AssessmentRepository AssessmentRepository
        {
            get
            {
                if (_assessmentRepository == null)
                    _assessmentRepository = new AssessmentRepository(_db);
                return _assessmentRepository;
            }
        }

        public LessonTimesRepository LessonTimesRepository
        {
            get
            {
                if (_lessonTimesRepository == null)
                    _lessonTimesRepository = new LessonTimesRepository(_db);
                return _lessonTimesRepository;
            }
        }
        public async Task<int> Save()
        {
            return await _db.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
    }
}
