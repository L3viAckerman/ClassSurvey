using System.Security.Claims;
using ClassSurvey.Modules;
using ClassSurvey.Modules.MUsers.Entity;

namespace ClassSurvey
{
    public class MyPrincipal : ClaimsPrincipal
    {
        public MyPrincipal(UserEntity UserEntity)
        {
            this.UserEntity = UserEntity;
        }

        public UserEntity UserEntity { get; set; }
      
    }
}
