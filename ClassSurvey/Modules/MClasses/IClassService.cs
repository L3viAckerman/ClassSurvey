using ClassSurvey.Modules.MClasses.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MClasses
{
    public interface IClassService : ITransientService
    {
        int Count(UserEntity userEntity, ClassSearchEntity classSearchEntity);
        List<ClassEntity> List(UserEntity userEntity, ClassSearchEntity classSearchEntity);
        ClassEntity Get(UserEntity userEntity, Guid ClassId);
        ClassEntity Update(UserEntity userEntity, Guid ClassId, ClassEntity classEntity);
        bool Delete(UserEntity userEntity, Guid ClassId);
        ClassEntity Create(byte[] data);
        float CountSurvey(UserEntity UserEntity, Guid ClassId);
    }
}
