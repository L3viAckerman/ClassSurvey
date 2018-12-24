using ClassSurvey1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassForm1.Entities;

namespace ClassSurvey.Entities
{
    public partial class StudentClassEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }

        public ClassEntity Class { get; set; }
        public StudentEntity Student { get; set; }
        public ICollection<FormEntity> Forms { get; set; }
        public StudentClassEntity() : base() { }
        public StudentClassEntity(StudentClass studentClass, params object[] args) : base(studentClass)
        {
            foreach (var arg in args)
            {
                if (arg is Student student)
                {
                    this.Student = new StudentEntity(student);
                }
                else if (arg is Class Class)
                {
                    this.Class = new ClassEntity(Class);
                }
                else if (arg is ICollection<Form> Forms)
                {
                    this.Forms = Forms.Select(f => new FormEntity(f)).ToList();
                }
            }
        }
    }
    public partial class StudentClassSearchEntity : FilterEntity
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
    }
}
