using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;

namespace ClassSurvey.Modules.MUsers
{
    public interface IUserService : ITransientService, ICommonService
    {
        long Count(SearchUserEntity SearchUserEntity);
        List<UserEntity> List(SearchUserEntity SearchUserEntity);
        UserEntity Get(Guid UserId);
        bool ChangePassword(Guid UserId, PasswordChangeEntity passwordEntity);
        UserEntity Create(UserEntity UserEntity);
        UserEntity Update(Guid UserId, UserEntity UserEntity);
        bool Delete(Guid UserId);
        string Login(UserEntity UserEntity);
    }
}
