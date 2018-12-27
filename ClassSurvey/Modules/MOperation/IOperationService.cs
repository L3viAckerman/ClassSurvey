using ClassSurvey.Modules.MOperation.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MOperation
{
    public interface IOperationService : ITransientService
    {
        long Count(UserEntity UserEntity, SearchOperationEntity SearchOperationEntity);
        List<OperationEntity> Get(UserEntity UserEntity, SearchOperationEntity SearchOperationEntity);
        OperationEntity Get(UserEntity UserEntity, Guid OperationId);
        OperationEntity Create(UserEntity UserEntity, OperationEntity OperationEntity);
        OperationEntity Update(UserEntity UserEntity, Guid OperationId, OperationEntity OperationEntity);
        bool Delete(UserEntity UserEntity, Guid OperationId);
    }
}
