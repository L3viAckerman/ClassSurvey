
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassSurvey.Models;

namespace ClassSurvey.Modules
{
    public class UserEntity : BaseEntity
    {
        public Guid Id { get; set; }
        [Column(1)]
        public string Username { get; set; }
        [Column(2)]
        public string Password { get; set; }
        [Column(3)]
        public List<string> Roles { get; set; }
        public UserEntity() { }

        public UserEntity(User User) : base(User)
        {
            ROLES Roles = ROLES.USER;
            if (User.Admin!= null) Roles = Roles | ROLES.ADMIN;
            if (User.Student != null) Roles |= ROLES.STUDENT;
            if (User.Lecturer != null) Roles |= ROLES.LECTURER;
            
            this.Roles = Roles.ToString().Replace(" ", "").Split(",").ToList();
        }
        
    }

    public class PasswordChangeEntity
    {
        public string OldPassword { get; set; }
        
        public UserEntity UserEntity { get; set; }
        public PasswordChangeEntity(){}
    }
    public class SearchUserEntity : FilterEntity
    {
        public string Username { get; set; }
        //public IQueryable<User> ApplyTo(IQueryable<User> Users)
        //{
        //    if (!string.IsNullOrEmpty(Username))
        //        Users = Users.Where(u => u.Username.ToLower().Equals(Username.ToLower()));
        //    return Users;
        //}
    }
    
    [Flags]
    public enum ROLES
    {
        NONE = 0,
        USER = 1,
        ADMIN = 2,
        LECTURER = 4,
        STUDENT = 8,
    }
}
