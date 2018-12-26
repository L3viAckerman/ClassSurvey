using ClassSurvey.Modules;
using ClassSurvey.Modules.MUsers.Entity;

namespace ClassSurvey.Models
{
    public partial class User : Base
    {
        public User() : base() { }
        public User(UserEntity userEntity): base(userEntity)
        {
            
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is User User)
            {
                return Id.Equals(User.Id);
            }

            return false;
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is User User)
            {
                return Id.Equals(User.Id);
            }

            return false;
        }
    }
}