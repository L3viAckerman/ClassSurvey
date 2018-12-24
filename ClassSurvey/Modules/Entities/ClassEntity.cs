using ClassSurvey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using ClassSurvey.Models;

namespace ClassSurvey.Entities
{
    public partial class ClassEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public string ClassCode { get; set; }
        public int StudentNumber { get; set; }
        public Guid LecturerId { get; set; }
        public Guid VersionSurveyId { get; set; }
        public string Subject { get; set; }
        public string M { get; set; }
        public string M1 { get; set; }
        public string M2 { get; set; }
        public string Std { get; set; }
        public string Std1 { get; set; }
        public string Std2 { get; set; }
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public LecturerEntity Lecturer { get; set; }
        public ICollection<StudentClassEntity> StudentClasses { get; set; }
        public VersionSurveyEntity VersionSurveyEntity { get; set; }
        public string Semester { get; set; }
        public ClassEntity(Models.Class @class) : base() { }
        public ClassEntity(Class Class, params object[] args) : base(Class)
        {
            foreach (var arg in args)
            {
                if (arg is Lecturer lecturer)
                {
                    this.Lecturer = new LecturerEntity(lecturer);
                }
                else if (arg is ICollection<StudentClass> students)
                {
                    this.StudentClasses = students.Select(sc => new StudentClassEntity(sc,sc.Forms,sc.Student)).ToList();
                }
                else if (arg is VersionSurvey versionSurvey)
                {
                    this.VersionSurveyEntity = new VersionSurveyEntity(versionSurvey);
                }
            }
        }
    }
    public partial class ClassSearchEntity : FilterEntity
    {
        public Guid Id { get; set; }
        public string ClassCode { get; set; }
        public Guid LecturerId { get; set; }
        public string Subject { get; set; }
        public Guid VersionId { get; set; }
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Semester { get; set; }
    }
}
