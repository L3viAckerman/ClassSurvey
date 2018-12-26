using ClassSurvey.Modules.MUsers.Entity;
using ClassSurvey.Modules.MVersionSurveys.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MVersionSurveys
{
    public interface IVersionSurveyService : ITransientService
    {
        int Count(UserEntity userEntity, VersionSurveySearchEntity VersionSurveySearchEntity);
        List<VersionSurveyEntity> List(UserEntity userEntity, VersionSurveySearchEntity VersionSurveySearchEntity);
        VersionSurveyEntity Get(UserEntity userEntity, Guid VersionSurveyId);
        VersionSurveyEntity Update(UserEntity userEntity, Guid VersionSurveyId, VersionSurveyEntity VersionSurveyEntity);
        bool Delete(UserEntity userEntity, Guid VersionSurveyId);
        VersionSurveyEntity Create(UserEntity userEntity, VersionSurveyEntity versionSurveyEntity);
    }
}
