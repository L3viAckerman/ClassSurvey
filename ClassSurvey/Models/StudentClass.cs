using System;
using System.Collections.Generic;

namespace ClassSurvey1.Models
{
    public partial class StudentClass
    {
        public StudentClass()
        {
            Forms = new HashSet<Form>();
        }

        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }

        public Class Class { get; set; }
        public Student Student { get; set; }
        public ICollection<Form> Forms { get; set; }
    }
}
