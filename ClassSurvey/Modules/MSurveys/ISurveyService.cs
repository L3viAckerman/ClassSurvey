using ClassSurvey.Modules.MSurveys.Entity;
using ClassSurvey.Modules.MUsers.Entity;

namespace ClassSurvey.Modules.MSurveys
{
    public interface ISurveyService : ITransientService
    {
        void CreateOrUpdate(UserEntity userEntity, SurveyEntity SurveyEntity);
    }
}
