using ClassSurvey.Modules.MSurveys.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MSurveys
{
    public interface ISurveyService : ITransientService
    {
        void CreateOrUpdate(UserEntity userEntity, SurveyEntity SurveyEntity);
    }
}
