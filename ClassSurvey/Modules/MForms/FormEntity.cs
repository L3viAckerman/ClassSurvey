using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ClassSurvey;
using ClassSurvey.Models;
using ClassSurvey.Modules.MStudentClass.Entity;

namespace ClassSurvey.Modules.MForms.Entity
{
    public partial class FormEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid StudentClassId { get; set; }
        public string Content { get; set; }

        public Dictionary<string, string> ContentValues { get; set; }
        public StudentClassEntity StudentClass { get; set; }

        public FormEntity() : base()
        {

        }
        public FormEntity(Form Form, params object[] args) : base(Form)
        {
            if (this.Content != null || this.Content != "")
                this.ContentValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.Content);
            foreach (var arg in args)
            {
                if (arg is StudentClass studentClass)
                {
                    this.StudentClass = new StudentClassEntity(studentClass);
                }

            }
        }
    }

    public partial class FormSearchEntity : FilterEntity
    {
        public Guid Id { get; set; }
        public Guid StudentClassId { get; set; }

    }
}
