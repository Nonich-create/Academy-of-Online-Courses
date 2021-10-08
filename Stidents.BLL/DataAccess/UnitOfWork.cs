using Students.DAL.Models;
using System;
using System.Threading.Tasks;
using Students.BLL.Repository;

namespace Students.BLL.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _db;
        private Repository.CourseRepository _courseRepository;
        private Repository.ApplicationUserRepository _applicationUserRepository;
        private Repository.StudentRepository _studentRepository;
        private Repository.TeacherRepository _teacherRepository;
        private Repository.GroupRepository _groupRepository;
        private Repository.ManagerRepository _managerRepository;
        private Repository.LessonRepository _lessonRepository;
        private Repository.AssessmentRepository _assessmentRepository;
        private Repository.CourseApplicationRepository _courseApplicationRepository;
        private Repository.LessonTimesRepository _lessonTimesRepository;
        private bool disposed = false;

        public UnitOfWork(Context db)
        {
            _db = db;
        }

        public Repository.CourseRepository CourseRepository
        {
            get
            {
                if (_courseRepository == null)
                    _courseRepository = new Repository.CourseRepository(_db);
                return _courseRepository;
            }
        }
        public Repository.CourseApplicationRepository CourseApplicationRepository
        {
            get
            {
                if (_courseApplicationRepository == null)
                    _courseApplicationRepository = new Repository.CourseApplicationRepository(_db);
                return _courseApplicationRepository;
            }
        }

        public Repository.ManagerRepository ManagerRepository
        {
            get
            {
                if (_managerRepository == null)
                    _managerRepository = new Repository.ManagerRepository(_db);
                return _managerRepository;
            }
        }

        public Repository.GroupRepository GroupRepository
        {
            get
            {
                if (_groupRepository == null)
                    _groupRepository = new Repository.GroupRepository(_db);
                return _groupRepository;
            }
        }

        public Repository.ApplicationUserRepository ApplicationUsersRepository
        {
            get
            {
                if (_applicationUserRepository == null)
                    _applicationUserRepository = new Repository.ApplicationUserRepository(_db);
                return _applicationUserRepository;
            }
        }

        public Repository.StudentRepository StudentRepository
        {
            get
            {
                if (_studentRepository == null)
                    _studentRepository = new Repository.StudentRepository(_db);
                return _studentRepository;
            }
        }

        public Repository.TeacherRepository TeacherRepository
        {
            get
            {
                if (_teacherRepository == null)
                    _teacherRepository = new Repository.TeacherRepository(_db);
                return _teacherRepository;
            }
        }
        public Repository.LessonRepository LessonRepository
        {
            get
            {
                if (_lessonRepository == null)
                    _lessonRepository = new Repository.LessonRepository(_db);
                return _lessonRepository;
            }
        }
 
        public Repository.AssessmentRepository AssessmentRepository
        {
            get
            {
                if (_assessmentRepository == null)
                    _assessmentRepository = new Repository.AssessmentRepository(_db);
                return _assessmentRepository;
            }
        }

        public Repository.LessonTimesRepository LessonTimesRepository
        {
            get
            {
                if (_lessonTimesRepository == null)
                    _lessonTimesRepository = new Repository.LessonTimesRepository(_db);
                return _lessonTimesRepository;
            }
        }

      

        public async Task<int> SaveAsync()
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
