using ClassSurvey.Models;
using ClassSurvey.Modules.MOperation.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MOperation
{
    public class OperationService : CommonService, IOperationService
    {
        public OperationService() : base()
        {
        }
        public long Count(UserEntity UserEntity, SearchOperationEntity SearchOperationEntity)
        {
            if (SearchOperationEntity == null) SearchOperationEntity = new SearchOperationEntity();
            IQueryable<Operation> Operations = context.Operations;
            Operations = SearchOperationEntity.ApplyTo(Operations);
            return Operations.Count();
        }
        public List<OperationEntity> Get(UserEntity UserEntity, SearchOperationEntity SearchOperationEntity)
        {
            if (SearchOperationEntity == null) SearchOperationEntity = new SearchOperationEntity();
            IQueryable<Operation> Operations = context.Operations;
            Operations = SearchOperationEntity.ApplyTo(Operations);
            Operations = SearchOperationEntity.SkipAndTake(Operations);
            return Operations.ToList().Select(u => new OperationEntity(u)).ToList();
        }
        public OperationEntity Get(UserEntity UserEntity, Guid LecturerId)
        {
            Operation Operation = context.Operations.Where(u => u.Id == LecturerId).FirstOrDefault();
            if (Operation == null)
                throw new BadRequestException("Operations không tồn tại");
            return new OperationEntity(Operation);
        }
        public OperationEntity Create(UserEntity UserEntity, OperationEntity OperationEntity)
        {
            OperationEntity.Id = Guid.NewGuid();
            Operation Operation = OperationEntity.ToModel();
            context.Operations.Add(Operation);
            context.SaveChanges();
            return OperationEntity;
        }
        public OperationEntity Update(UserEntity UserEntity, Guid OperationId, OperationEntity OperationEntity)
        {
            Operation Operation = context.Operations.Where(l => l.Id == OperationId).FirstOrDefault();
            OperationEntity.ToModel(Operation);
            context.SaveChanges();
            return OperationEntity;
        }
        public bool Delete(UserEntity UserEntity, Guid OperationId)
        {
            Operation Operation = context.Operations.Where(l => l.Id == OperationId).FirstOrDefault();
            if (Operation == null)
                throw new BadRequestException("Operation not exist!");
            context.Operations.Remove(Operation);
            context.SaveChanges();
            return true;
        }

    }
}
