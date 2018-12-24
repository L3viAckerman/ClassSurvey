using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClassSurvey.Modules
{
    public class CommonController : Controller
    {
        public UserEntity UserEntity => (User as MyPrincipal)?.UserEntity;
    }
}