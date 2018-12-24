using System.Security.Claims;
using ClassSurvey.Modules;


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
