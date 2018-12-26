using System;
using System.Collections.Generic;

namespace ClassSurvey.Modules.MSurveys.Entity
{
    public class SurveyEntity : BaseEntity
    {
        public DateTime OpenedDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public List<Guid> ClassGuids { get; set; }
        public Guid VersionSurveyId { get; set; }
    }
}
