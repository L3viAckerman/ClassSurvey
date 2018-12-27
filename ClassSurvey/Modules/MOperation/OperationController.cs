using ClassSurvey.Modules.MOperation.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MOperation
{
    [Route("api/Operation")]
    public class OperationController : CommonController
    {
        public IOperationService OperationService;
        public OperationController(IOperationService OperationService)
        {
            this.OperationService = OperationService;
        }

        [Route("Count"), HttpGet]
        public long Count(SearchOperationEntity SearchOperationEntity)
        {
            return OperationService.Count(UserEntity, SearchOperationEntity);
        }

        [Route(""), HttpGet]
        public List<OperationEntity> Get(SearchOperationEntity SearchOperationEntity)
        {
            return OperationService.Get(UserEntity, SearchOperationEntity);
        }
        [Route("{LecturerId}"), HttpGet]
        public OperationEntity Get(Guid OperationId)
        {
            return OperationService.Get(UserEntity, OperationId);
        }
        [Route(""), HttpPost]
        public OperationEntity Create([FromBody]OperationEntity OperationEntity)
        {
            return OperationService.Create(UserEntity, OperationEntity);
        }
        [Route("{OperationId}"), HttpPut]
        public OperationEntity Update(Guid OperationId, [FromBody]OperationEntity OperationEntity)
        {
            return OperationService.Update(UserEntity, OperationId, OperationEntity);
        }
        [Route("{OperationId}"), HttpDelete]
        public bool Delete(Guid OperationId)
        {
            return OperationService.Delete(UserEntity, OperationId);
        }
    }
}
