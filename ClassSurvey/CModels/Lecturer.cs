using ClassSurvey1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Models
{
    public partial class Lecturer : Base
    {
        
        public Lecturer(LecturerEntity lecturerEntity) : base(lecturerEntity)
        {
            if(lecturerEntity.Classes != null)
            {
                this.Classes = new HashSet<Class>();
                foreach(var classEntity in lecturerEntity.Classes)
                {
                    classEntity.LecturerId = this.Id;
                    this.Classes.Add(new Class(classEntity));
                }
                
            }
            
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is Lecturer lecturer)
            {
                return Id.Equals(lecturer.Id) && LecturerCode.Equals(lecturer.LecturerCode);
            }

            return false;
        }
        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (other is Lecturer lecturer)
            {
                return Id.Equals(lecturer.Id) && LecturerCode.Equals(lecturer.LecturerCode);
            }

            return false;
        }
    }
}
