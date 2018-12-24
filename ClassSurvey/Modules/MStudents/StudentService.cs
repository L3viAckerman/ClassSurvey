using System;
using System.Collections.Generic;
using System.Linq;
using ClassSurvey.Entities;
using ClassSurvey.Models;
using ClassSurvey.Modules.MUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ClassSurvey.Modules.MStudents
{
    

    public class StudentService : CommonService, IStudentService
    {
        public IUserService UserService;

        public StudentService(IUserService userService)
        {
            this.UserService = userService;
        }

        public int Count(UserEntity userEntity, StudentSearchEntity StudentSearchEntity)
        {
            if (StudentSearchEntity == null) StudentSearchEntity = new StudentSearchEntity();
            IQueryable<Student> Students = context.Students;
            Students = Apply(Students, StudentSearchEntity);
            return Students.Count();
        }

        public List<StudentEntity> List(UserEntity userEntity, StudentSearchEntity StudentSearchEntity)
        {
            if (StudentSearchEntity == null) StudentSearchEntity = new StudentSearchEntity();
            IQueryable<Student> Students = context.Students.Include(s=>s.StudentClasses);
            Students = Apply(Students, StudentSearchEntity);
//            List<User> Users = new List<User>();
//            foreach (var Student in Students)
//            {
//                var User = context.Users.FirstOrDefault(u => u.Id == Student.Id);
//                Users.Add(User);
//            }
//            
//            return Students.Join(Users, u => u.Id, s => s.Id, (student, user) => new StudentEntity(student, student.StudentClasses, user)).ToList();
            //Students = StudentSearchEntity.SkipAndTake(Students);
            return Students.Select(s => new StudentEntity(s, s.StudentClasses)).ToList();

        }
        public List<ClassEntity> GetClasses(Guid StudentId)
        {
            List<StudentClass> studentClasses = context.StudentClasses.Where(sc=>sc.StudentId == StudentId).ToList();
            if(studentClasses == null) throw new NotFoundException("Class Not Found");
            List<ClassEntity> result = new List<ClassEntity>();
            foreach (var sc in studentClasses)
            {
                var Class = context.Classes.Include(c => c.VersionSurvey).Include(c=>c.Lecturer).Include(c => c.StudentClasses).ThenInclude(s=>s.Forms)
                    .FirstOrDefault(c => c.Id == sc.ClassId);
                result.Add(new ClassEntity(Class, Class.VersionSurvey, Class.StudentClasses, Class.Lecturer));
            }

            return result;
        }
        
        public StudentEntity Get(UserEntity userEntity, Guid StudentId)
        {
            Student Student = context.Students.Include(s=>s.StudentClasses).ThenInclude(sc=>sc.Forms).FirstOrDefault(c => c.Id == StudentId); ///add include later
            //User User = context.Users.FirstOrDefault(u => u.Id == StudentId);
            if (Student == null) throw new NotFoundException("Student Not Found");
            return new StudentEntity(Student,Student.StudentClasses);
        }

        public StudentEntity Update(UserEntity userEntity, Guid StudentId, StudentEntity StudentEntity)
        {
            Student Student = context.Students.Include(s=>s.StudentClasses).FirstOrDefault(c => c.Id == StudentId); //add include later
            if (Student == null) throw new NotFoundException("Student Not Found");
            Student updateStudent = new Student(StudentEntity);
            updateStudent.CopyTo(Student);
            context.SaveChanges();
            List<StudentClass> studentClasses = context.StudentClasses.Where(sc => sc.StudentId == StudentId).ToList();
            List<StudentClass> Insert, Update, Delete;
            List<StudentClass> newStudentClasses = StudentEntity.StudentClasses == null
                ? new List<StudentClass>()
                : StudentEntity.StudentClasses.Select(sc => new StudentClass(sc)).ToList();
            Common<StudentClass>.Split(newStudentClasses, studentClasses, out Insert, out Update, out Delete);
            if (Insert != null)
                foreach (var sc in Insert)
                {
                    sc.Id = Guid.NewGuid();
                    sc.StudentId = Student.Id;
                    studentClasses.Add(sc);
                    context.StudentClasses.Add(sc);
                }

            if (Update != null)
                foreach (var sc in Update)
                {
                    var curClass = studentClasses.FirstOrDefault(s => s.Id == sc.Id);
                    Common<StudentClass>.Copy(sc, curClass);
                }

            if (Delete != null)
                foreach (var sc in Delete)
                {
                    var deleteClass = studentClasses.FirstOrDefault(s => sc.Id == s.Id);
                    foreach (var form in context.Forms.Where(f => f.StudentClassId == deleteClass.Id).ToList())
                    {
                        context.Forms.Remove(form);
                    }

                    context.SaveChanges();
                    context.StudentClasses.Remove(deleteClass);
                }

            context.SaveChanges();

            return new StudentEntity(Student, Student.StudentClasses);
        }

        public bool Delete(UserEntity userEntity, Guid StudentId)
        {
            var CurrentStudent = context.Students.FirstOrDefault(c => c.Id == StudentId);
            var StudentClasses = context.StudentClasses.Include(f=>f.Forms).Where(sc => sc.StudentId == StudentId);
            if (StudentClasses != null)
            {
                foreach (var studentClass in StudentClasses)
                {
                    foreach (var form in studentClass.Forms)
                    {
                        context.Forms.Remove(form);
                    }
                    context.StudentClasses.Remove(studentClass);
                }
            }
            
            var User = context.Users.FirstOrDefault(u => u.Id == StudentId);
            if (User == null) return false;
            if (CurrentStudent == null) return false;
            
            context.Students.Remove(CurrentStudent);
            context.SaveChanges();
            context.Users.Remove(User);
            context.SaveChanges();
            return true;
        }

        public List<StudentEntity> CreateFromExcel(byte[] data)
        {
            List<StudentEntity> StudentEntities = new List<StudentEntity>();
            if (data != null)
            {
                List<StudentExcelModel> StudentExcelModels =
                    ConvertToIEnumrable<StudentExcelModel>(data).ToList();

                foreach (var StudentExcelModel in StudentExcelModels.Where(sem => sem.Name != null))
                {
                    //Create user 
                    var userEntity = new UserEntity();
                    userEntity.Password = StudentExcelModel.Password;
                    userEntity.Username = StudentExcelModel.UserName.Trim();
                    UserService.Create(userEntity);
                    context.SaveChanges();
                    var users = context.Users.Where(u => u.Username == userEntity.Username).ToList();
                    if(users.Count > 1) throw new BadRequestException("Trung sinh vien co MSSV la " + userEntity.Username);
                    var user = users.First();
                    user.Role = 8;
                    context.SaveChanges();
                    //Create Entity 
                    var newStudentEntity = new StudentEntity();
                    newStudentEntity = StudentExcelModel.ToEntity(newStudentEntity);
                    newStudentEntity.Id = user.Id;
                    var newStudent = new Student(newStudentEntity);
                    context.Students.Add(newStudent);
                    
                    StudentEntities.Add(new StudentEntity(newStudent));
                }
                context.SaveChanges();
            }

            return StudentEntities;
        }

        public StudentEntity Create(StudentExcelModel StudentExcelModel)
        {
            var userEntity = new UserEntity();
            userEntity.Password = StudentExcelModel.Password;
            userEntity.Username = StudentExcelModel.UserName.Trim();
            UserService.Create(userEntity);
            var users = context.Users.Where(u => u.Username == userEntity.Username).ToList();
            if(users.Count > 1) throw new BadRequestException("Trung sinh vien");
            var user = users.FirstOrDefault();
            user.Role = 8;
            context.SaveChanges();
            //Create User 
            var newStudentEntity = new StudentEntity();
            newStudentEntity = StudentExcelModel.ToEntity(newStudentEntity);
            newStudentEntity.Id = user.Id;
            var newStudent = new Student(newStudentEntity);
            context.Students.Add(newStudent);
            context.SaveChanges();
            return newStudentEntity;
        }

        private IQueryable<Student> Apply(IQueryable<Student> Students, StudentSearchEntity StudentSearchEntity)
        {
            if (StudentSearchEntity.Name != null)
            {
                Students = Students.Where(l =>
                    l.Name.ToLower().Contains(StudentSearchEntity.Name.ToLower()) || StudentSearchEntity.Name.ToLower().Contains(l.Name.ToLower()));
            }

            if (StudentSearchEntity.Code != null)
            {
                Students = Students.Where(l =>
                    l.Code == StudentSearchEntity.Code);
            }
            if (StudentSearchEntity.Username != null)
            {
                Students = Students.Where(l =>
                    l.Username.ToLower().Contains(StudentSearchEntity.Username.ToLower()) 
                    || StudentSearchEntity.Username.ToLower().Contains(l.Username.ToLower()));
            }
            if (StudentSearchEntity.Class != null)
            {
                Students = Students.Where(l =>
                    l.Class.ToLower().Equals(StudentSearchEntity.Class.ToLower()));
            }
            return Students;
        }
    }

    
}