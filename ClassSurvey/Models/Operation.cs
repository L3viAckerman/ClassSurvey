using System;
using System.Collections.Generic;

namespace ClassSurvey1.Models
{
    public partial class Operation
    {
        public Guid Id { get; set; }
        public string Link { get; set; }
        public int? Role { get; set; }
        public string Method { get; set; }
    }
}
