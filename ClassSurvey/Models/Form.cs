using System;
using System.Collections.Generic;

namespace ClassSurvey1.Models
{
    public partial class Form
    {
        public Guid Id { get; set; }
        public Guid StudentClassId { get; set; }
        public string Content { get; set; }

        public StudentClass StudentClass { get; set; }
    }
}
