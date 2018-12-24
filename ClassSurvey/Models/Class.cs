using System;
using System.Collections.Generic;

namespace ClassSurvey.Models
{
    public partial class Class
    {
        public Class()
        {
            StudentClasses = new HashSet<StudentClass>();
        }

        public Guid Id { get; set; }
        public int StudentNumber { get; set; }
        public string Subject { get; set; }
        public string ClassCode { get; set; }
        public Guid LecturerId { get; set; }
        public string M1 { get; set; }
        public string M2 { get; set; }
        public string M { get; set; }
        public string Std2 { get; set; }
        public string Std { get; set; }
        public string Std1 { get; set; }
        public Guid VersionSurveyId { get; set; }
        public string Semester { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? OpenedDate { get; set; }

        public Lecturer Lecturer { get; set; }
        public VersionSurvey VersionSurvey { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; }
    }
}
