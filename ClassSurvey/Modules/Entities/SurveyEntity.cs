using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Entities
{
    public class SurveyEntity : BaseEntity 
    {
        public DateTime OpenedDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public List<Guid> ClassGuids { get; set; }
        public Guid VersionSurveyId { get; set; }
    }
}
