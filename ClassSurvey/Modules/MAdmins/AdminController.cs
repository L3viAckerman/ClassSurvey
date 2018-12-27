using System;
using System.Collections.Generic;
using ClassSurvey.Modules;
using ClassSurvey.Modules.MAdmins;
using ClassSurvey.Modules.MAdmins.Entity;
using ClassSurvey.Modules.MClasses;
using Microsoft.AspNetCore.Mvc;

namespace ClassSurvey.Modules.MAdmins
{
    [Route("api/Admins")]
    public class AdminController : CommonController
    {
        private IAdminService AdminService;

        public AdminController(IAdminService AdminService)
        {
            this.AdminService = AdminService;
        }       
        
        
        [HttpPut("{AdminId}")]
        public AdminEntity Update([FromBody] AdminEntity AdminEntity, [FromRoute]Guid AdminId)
        {
            return AdminService.Update(UserEntity, AdminId, AdminEntity);
        }
        [HttpGet("{AdminId}")]
        public AdminEntity Get([FromRoute]Guid AdminId)
        {
            return AdminService.Get(UserEntity, AdminId);
        }
        [HttpGet("Count")]
        public int Count([FromBody]AdminSearchEntity AdminSearchEntity)
        {
            return AdminService.Count(UserEntity, AdminSearchEntity);
        }
        [HttpGet("List")]
        public List<AdminEntity> List([FromBody]AdminSearchEntity AdminSearchEntity)
        {
            return AdminService.List(UserEntity, AdminSearchEntity);
        }
        [HttpDelete("{AdminId}")]
        public bool Delete([FromRoute]Guid AdminId)
        {
            return AdminService.Delete(UserEntity, AdminId);
        }

        [HttpPost]
        public AdminEntity Create([FromBody]AdminDto AdminDto)
        {
            return AdminService.Create(UserEntity, AdminDto);
        }
        
    }
}