using ClassSurvey.Entities;
using ClassSurvey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ClassSurvey.Models
{
    public partial class Class : Base
    {
       
        public Class(ClassEntity classEntity) : base(classEntity)
        {
            if(classEntity.Lecturer != null)
            {
                this.Lecturer = new Lecturer(classEntity.Lecturer);
            }
            if(classEntity.StudentClasses != null)
            {
                this.StudentClasses = new HashSet<StudentClass>();
                foreach(var studentClassEntity in classEntity.StudentClasses)
                {
                    studentClassEntity.ClassId = this.Id;
                    this.StudentClasses.Add(new StudentClass(studentClassEntity));
                }
            }

            if (classEntity.VersionSurveyEntity != null)
            {
                this.VersionSurvey = new VersionSurvey(classEntity.VersionSurveyEntity);
            }
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is Class Class)
            {
                return Id.Equals(Class.Id) && ClassCode.Equals(Class.ClassCode);
            }

            return false;
        }
        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (other is Class Class)
            {
                return Id.Equals(Class.Id) && ClassCode.Equals(Class.ClassCode);
            }

            return false;
        }
    }
}
