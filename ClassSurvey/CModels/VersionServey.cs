using ClassSurvey.Modules.MVersionSurveys.Entity;

namespace ClassSurvey.Models
{
    public partial class VersionSurvey : Base
    {
        //public VersionSurvey() : base() { }
        public VersionSurvey(VersionSurveyEntity versionSurveyEntity) : base(versionSurveyEntity)
        {
            
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is VersionSurvey VersionSurvey)
            {
                return Id.Equals(VersionSurvey.Id);
            }

            return false;
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is VersionSurvey VersionSurvey)
            {
                return Id.Equals(VersionSurvey.Id);
            }

            return false;
        }
    }
}
