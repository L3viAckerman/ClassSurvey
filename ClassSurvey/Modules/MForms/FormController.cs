using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassSurvey.Modules.MClasses;
using ClassSurvey.Modules.MForms.Entity;
using ClassSurvey.Modules.MSurveys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassSurvey.Modules.MForms
{
    [Route("api/Forms")]
    public class FormController : CommonController
    {
        private IFormService FormService;

        public FormController(IFormService FormService)
        {
            this.FormService = FormService;
        }


        [HttpGet("Count")]
        public int Count(FormSearchEntity FormSearchEntity)
        {
            return FormService.Count(UserEntity, FormSearchEntity);
        }
        [HttpGet("List")]
        public List<FormEntity> List(FormSearchEntity FormSearchEntity)
        {
            return FormService.List(UserEntity, FormSearchEntity);
        }
        [HttpPut("{FormId}")]
        public FormEntity Update([FromBody] FormEntity FormEntity, [FromRoute]Guid FormId)
        {
            return FormService.Update(UserEntity, FormId, FormEntity);
        }
        [HttpGet("{FormId}")]
        public FormEntity Get([FromRoute]Guid FormId)
        {
            return FormService.Get(UserEntity, FormId);
        }
        [HttpDelete("{FormId}")]
        public bool Delete([FromRoute]Guid FormId)
        {
            return FormService.Delete(UserEntity, FormId);
        }

        [HttpPost]
        public FormEntity Create([FromBody]FormEntity FormEntity)
        {
            return FormService.Create(UserEntity, FormEntity);
        }

    }
}