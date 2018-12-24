using System;
using System.Collections.Generic;

namespace ClassSurvey1.Models
{
    public partial class Admin
    {
        public Guid Id { get; set; }
        public string Vnumail { get; set; }
        public string Name { get; set; }
        public int? Role { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }

        public User IdNavigation { get; set; }
    }
}
