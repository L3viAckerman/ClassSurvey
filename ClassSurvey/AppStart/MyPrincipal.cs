using System.Security.Claims;
using ClassSurvey1.Modules;


namespace ClassSurvey1
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
