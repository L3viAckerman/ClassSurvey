using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassSurvey.Modules.MUsers;
using ClassSurvey.Modules.MUsers.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassSurvey.Modules
{
    [Route("api/Users")]
    public class UserController : CommonController
    {
        public IUserService UserService;
        
        public UserController(IUserService UserService)
        {
            this.UserService = UserService;
           
        }

        [Route("Count"), HttpGet]
        public long Count(SearchUserEntity SearchUserEntity)
        {
            return UserService.Count(SearchUserEntity);
        }

        [Route("List"), HttpGet]
        public List<UserEntity> List(SearchUserEntity SearchUserEntity)
        {
            return UserService.List(SearchUserEntity);
        }
        [Route("{UserId}"), HttpGet]
        public UserEntity Get([FromRoute]Guid UserId)
        {
            return UserService.Get(UserId);
        }
        [Route(""), HttpPost]
        public UserEntity Create([FromBody]UserEntity UserEntity)
        {
            return UserService.Create(UserEntity);
        }
        [Route("{UserId}"), HttpPut]
        public UserEntity Update(Guid UserId, [FromBody]UserEntity UserEntity)
        {
            return UserService.Update(UserId, UserEntity);
        }
        [Route("{UserId}/ChangePassword"), HttpPut]
        public bool ChangePassword(Guid UserId, [FromBody]PasswordChangeEntity PasswordEntity)
        {
            return UserService.ChangePassword(UserId, PasswordEntity);
        }
        [Route("{UserId}"), HttpDelete]
        public bool Delete(Guid UserId)
        {
            return UserService.Delete(UserId);
        }
        [Route("Login")]
        public IActionResult Login([FromBody]UserEntity UserEntity)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    string JWT = UserService.Login(UserEntity);
                    var CookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(30), Path = "/" };
                    Response.Cookies.Append("JWT", JWT, CookieOptions);
                    return Ok("Authentication Successful");
                    //return RedirectToPage("/");
                }
                //return RedirectToPage("/login");
                return Ok("Need to login first");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return Ok("Authentication failed");
            //return RedirectToPage("/login");// can doi 
        }
        [Route("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWT");
            return Ok("Success");
            //return RedirectToPage("/login");
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> InsertUsers([FromForm]UploadClass data)
        {
            IEnumerable<IFormFile> files = data.myFiles;
            foreach (var file in files)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    byte[] bytes = ms.ToArray();
                    UserService.ConvertToIEnumrable<UserEntity>(bytes);
                }
            }
            return Ok();
        }
    }
}