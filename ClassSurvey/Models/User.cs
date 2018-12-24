using System;
using System.Collections.Generic;

namespace ClassSurvey1.Models
{
    public partial class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int Role { get; set; }
        public string Password { get; set; }

        public Admin Admin { get; set; }
        public Lecturer Lecturer { get; set; }
        public Student Student { get; set; }
    }
}
