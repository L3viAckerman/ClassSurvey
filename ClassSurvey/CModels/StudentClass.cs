using ClassSurvey.Modules.MStudentClass.Entity;
using System;
using System.Collections.Generic;

namespace ClassSurvey.Models
{
    public partial class StudentClass : Base
    {
        
        public StudentClass(StudentClassEntity studentClassEntity): base(studentClassEntity)
        {
            if(studentClassEntity.Class != null)
            {
                this.Class = new Class(studentClassEntity.Class);
            }
            if(studentClassEntity.Student != null)
            {
                this.Student = new Student(studentClassEntity.Student);
            }
            if(studentClassEntity.Forms != null)
            {
                this.Forms = new HashSet<Form>();
                foreach(var form in studentClassEntity.Forms)
                {
                    form.StudentClassId = this.Id;
                    Forms.Add(new Form(form));
                }
            }
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is StudentClass StudentClass)
            {
                return this.ClassId == StudentClass.ClassId && this.StudentId == StudentClass.StudentId;
            }

            return false;
        }
        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (other is StudentClass StudentClass)
            {
                return this.Id.Equals(StudentClass.Id);
            }

            return false;
        }
    }
}
