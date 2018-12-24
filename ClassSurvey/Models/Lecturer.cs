using System;
using System.Collections.Generic;

namespace ClassSurvey.Models
{
    public partial class Lecturer
    {
        public Lecturer()
        {
            Classes = new HashSet<Class>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int? Role { get; set; }
        public string LecturerCode { get; set; }
        public string Vnumail { get; set; }
        public string Phone { get; set; }

        public User IdNavigation { get; set; }
        public ICollection<Class> Classes { get; set; }
    }
}
