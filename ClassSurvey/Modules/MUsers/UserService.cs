using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ClassSurvey.Models;
using ClassSurvey.Modules.MUsers;
using ClassSurvey.Modules.MUsers.Entity;

namespace ClassSurvey.Modules
{
    
    public class UserService : CommonService, IUserService
    {
        private IJWTHandler JWTHandler;
        private SecurePasswordHasher SecurePasswordHasher;
        public UserService(IJWTHandler JWTHandler) : base()
        {
            this.JWTHandler = JWTHandler;
            this.SecurePasswordHasher = new SecurePasswordHasher();
        }
        public long Count(SearchUserEntity SearchUserEntity)
        {
            if (SearchUserEntity == null) SearchUserEntity = new SearchUserEntity();
            IQueryable<User> Users = context.Users;
            Apply(Users, SearchUserEntity);
            return Users.Count();
        }
        public List<UserEntity> List(SearchUserEntity SearchUserEntity)
        {
            if (SearchUserEntity == null) SearchUserEntity = new SearchUserEntity();
            IQueryable<User> Users = context.Users
                .Include(u => u.Admin)
                .Include(u => u.Student)
                .Include(u => u.Lecturer);
                
            Apply(Users, SearchUserEntity);
            //Users = SearchUserEntity.SkipAndTake(Users);
            return Users.ToList().Select(u => new UserEntity(u)).ToList();
        }

        
        public UserEntity Get(Guid UserId)
        {
            User User =  context.Users
                .Include(u => u.Admin)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .Where(u => u.Id == UserId).FirstOrDefault();
            if (User == null)
                throw new BadRequestException("User not exist!");
            return new UserEntity(User);
        }

        public UserEntity Create(UserEntity UserEntity)
        {
            if (string.IsNullOrEmpty(UserEntity.Username))
                throw new BadRequestException("You didn't fill Username!");
            if (string.IsNullOrEmpty(UserEntity.Password))
                throw new BadRequestException("You didn't fill Password!");
            User User = context.Users.Where(u => u.Username.ToLower().Equals(UserEntity.Username.ToLower())).FirstOrDefault();
            if (User == null)
            {
                User = new User();
                User.Id = Guid.NewGuid();
                User.Username = UserEntity.Username;
                User.Password = SecurePasswordHasher.Hash(UserEntity.Password);
                context.Users.Add(User);
            }
            
            context.SaveChanges();
            UserEntity.Id = User.Id;
            UserEntity.Password = User.Password;
            return UserEntity;

        }
        public bool ChangePassword(Guid userId, PasswordChangeEntity passwordEntity)
        {
            
            User User = context.Users.FirstOrDefault(u => u.Id.Equals(userId));
            if (User == null) return false;
            if (SecurePasswordHasher.Verify(passwordEntity.OldPassword, User.Password))
            {
                //User newPasswordUser = new User(passwordEntity.UserEntity);
                User.Password = SecurePasswordHasher.Hash(passwordEntity.UserEntity.Password);
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public UserEntity Update(Guid UserId, UserEntity UserEntity)
        {
            User User = context.Users.FirstOrDefault(u => u.Id.Equals(UserEntity.Id));
            if (User == null)
                throw new BadRequestException("User not exist!");
            User.Username = UserEntity.Username.Trim(); 
            User.Password = SecurePasswordHasher.Hash(UserEntity.Password);
            context.SaveChanges();
            return new UserEntity(User);
        }
        public bool Delete(Guid UserId)
        {
            User User = context.Users.Where(u => u.Id == UserId).FirstOrDefault();
            if (User == null)
                throw new BadRequestException("User not exist!");
            context.Users.Remove(User);
            context.SaveChanges();
            return true;
        }

        public string Login(UserEntity UserEntity)
        {
            if (string.IsNullOrEmpty(UserEntity.Username))
                throw new BadRequestException("You didn't fill Username!");
            if (string.IsNullOrEmpty(UserEntity.Password))
                throw new BadRequestException("You didn't fill Password!");

            User User = context.Users
               .Include(u => u.Admin)
               .Include(u => u.Student)
               .Include(u => u.Lecturer)
               .Where(u => u.Username.ToLower().Equals(UserEntity.Username.ToLower())).FirstOrDefault();

            if (User == null)
                throw new BadRequestException("User not exist!");
            //string hashed1234 = SecurePasswordHasher.Hash("1234"); //hashed password 
            string hashPassword = (User.Password);
            if (!SecurePasswordHasher.Verify(UserEntity.Password, hashPassword))
                throw new BadRequestException("Wrong password!");
            UserEntity = new UserEntity(User);
            //UserEntity.Roles = new List<string>() { "1", "4" };
            return JWTHandler.CreateToken(UserEntity);
        }

        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            HashAlgorithm algorithm = SHA256.Create();
            byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private void Apply(IEnumerable<User> source, SearchUserEntity SearchUserEntity)
        {
            if (SearchUserEntity.Username != null)
            {
                source = source.Where(u =>
                    u.Username.Contains(SearchUserEntity.Username) || SearchUserEntity.Username.Contains(u.Username));
            }
        }
    }
}
