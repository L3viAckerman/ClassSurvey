using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassSurvey.Modules.MUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassSurvey.Modules
{
    [Authorize]
    [Route("App")]
    public class AppController : CommonController
    {
        private IUserService UserService;
        public AppController(IUserService UserService)
        {
            this.UserService = UserService;
        }
        //public IActionResult Index()
        //{
        //    var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
        //    return PhysicalFile(file, "text/html");
        //}

        [AllowAnonymous]
        [Route("Login")]
        public IActionResult Login([FromBody]UserEntity UserEntity)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    string JWT = UserService.Login(UserEntity);
                    Response.Cookies.Append("JWT", JWT);
                    return Ok("Authentication Successful");
                    //return RedirectToPage("/");
                }
                //return RedirectToPage("/login");
                return Ok("Need to login first");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
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
    }
}