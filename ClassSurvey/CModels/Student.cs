using ClassSurvey.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Models
{
    public partial class Student : Base
    {
        //public Student():base() {}
        public Student(StudentEntity studentEntity) : base(studentEntity)
        {
            if (studentEntity.StudentClasses != null)
            {
                this.StudentClasses = new HashSet<StudentClass>();
                foreach(var studentClassEntity in studentEntity.StudentClasses)
                {
                    studentClassEntity.StudentId = this.Id;
                    this.StudentClasses.Add(new StudentClass(studentClassEntity));
                }
            }
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is Student Student)
            {
                return Id.Equals(Student.Id) && Code.Equals(Student.Code);
            }

            return false;
        }
        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (other is Student Student)
            {
                return Id.Equals(Student.Id) && Code.Equals(Student.Code);
            }

            return false;
        }
    }
}
