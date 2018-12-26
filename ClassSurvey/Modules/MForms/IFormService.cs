using ClassSurvey.Modules.MForms.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;

namespace ClassSurvey.Modules.MForms
{
    public interface IFormService : ITransientService
    {
        int Count(UserEntity userEntity, FormSearchEntity FormSearchEntity);
        List<FormEntity> List(UserEntity userEntity, FormSearchEntity FormSearchEntity);
        FormEntity Get(UserEntity userEntity, Guid FormId);
        FormEntity Update(UserEntity userEntity, Guid FormId, FormEntity FormEntity);
        bool Delete(UserEntity userEntity, Guid FormId);
        FormEntity Create(UserEntity userEntity, FormEntity FormEntity);

    }
}
