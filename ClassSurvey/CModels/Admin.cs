using ClassSurvey.Models;
using ClassSurvey.Modules.MAdmins.Entity;
using System;


namespace ClassSurvey.Models
{
    public partial class Admin : Base
    {
        public Admin() : base() { }
        public Admin(AdminEntity adminEntity) : base(adminEntity)
        {

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(Base other)
        {
            if (other == null) return false;
            if (other is Admin admin)
            {
                return Id.Equals(admin.Id);
            }

            return false;
        }
        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (other is Admin admin)
            {
                return Id.Equals(admin.Id);
            }

            return false;
        }
    }
}
