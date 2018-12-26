using ClassSurvey.Modules.MSurveys.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MSurveys
{
    [Route("api/Surveys")]
    public class SurveyController : CommonController
    {
        private ISurveyService SurveyService;
        public SurveyController(ISurveyService SurveyService)
        {
            this.SurveyService = SurveyService;
        }
        [HttpPost]
        public IActionResult Create([FromBody]SurveyEntity SurveyEntity)
        {
            SurveyService.CreateOrUpdate(UserEntity, SurveyEntity);
            return Ok();
        }
        [HttpPut]
        public IActionResult Update([FromBody]SurveyEntity SurveyEntity)
        {
            SurveyService.CreateOrUpdate(UserEntity, SurveyEntity);
            return Ok();
        }
    }
}
