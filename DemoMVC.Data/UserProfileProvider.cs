using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class UserProfileProvider : BaseProvider
    {
        public UserProfileProvider()
        {

        }

        public UserProfile GetUserById(int UserId)
        {
            return _db.UserProfile.Where(a => a.UserId == UserId).FirstOrDefault();
        }
        public UserProfile GetUserByEmailId(string emailId)
        {
            return _db.UserProfile.Where(a => a.Email == emailId && a.IsActive && !a.IsDeleted).FirstOrDefault();
        }
        public List<UserProfile> GetAllUserProfile(string rolecode = "")
        {
            var getallusers = (from user in _db.UserProfile.AsEnumerable()
                               join uir in _db.webpages_UsersInRoles
                               on user.UserId equals uir.UserId
                               join role in _db.webpages_Roles
                               on uir.RoleId equals role.RoleId
                               where user.IsDeleted == false
                               && (rolecode == "" || role.RoleCode.ToUpper() == rolecode.ToUpper())
                               select new UserProfile
                               {
                                   UserId = user.UserId,
                                   UserName = user.UserName,
                                   Name = user.Name,
                                   Email = user.Email,
                                   IsActive = user.IsActive,
                                   Role = role.RoleName,
                                   RoleCode = role.RoleCode
                               }).Where(x => x.RoleCode.ToLower() != DemoMVC.Helper.Constants.RoleCode.SADMIN.ToLower()).OrderBy(x => x.Name).AsQueryable();

            return getallusers.ToList();
        }
        public IQueryable<UserProfileGridModel> GetAllUserProfileGrid()
        {
            return (from user in _db.UserProfile
                    join uir in _db.webpages_UsersInRoles on user.UserId equals uir.UserId
                    join role in _db.webpages_Roles on uir.RoleId equals role.RoleId
                    select new UserProfileGridModel
                    {
                        Id = user.UserId,
                        UserName = user.UserName,
                        Name = user.Name,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        Role = role.RoleName,
                        RoleCode = role.RoleCode,
                        UpdatedOn = user.UpdatedOn == null ? user.CreatedOn : user.UpdatedOn
                    }).AsQueryable();
        //            .Where(x => x.RoleCode.ToLower() != DemoMVC.Helper.Constants.RoleCode.SADMIN.ToLower()).AsQueryable();
        }
        public int UpdateUserProfile(UserProfile userprofile)
        {


            var getuser = GetUserById(userprofile.UserId);
            if (getuser != null)
            {
                getuser.UserName = userprofile.UserName;
                getuser.Name = userprofile.Name;
                getuser.Email = userprofile.Email;
                getuser.PhoneNo = userprofile.PhoneNo;
                getuser.MobileNo = userprofile.MobileNo;
                getuser.IsActive = userprofile.IsActive;
                getuser.UpdatedBy = userprofile.UpdatedBy;
                getuser.UpdatedOn = userprofile.UpdatedOn;
                getuser.IsDeleted = userprofile.IsDeleted;
                _db.Entry(getuser).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return userprofile.UserId;
        }

        public List<UserProfile> CheckDuplicateUserEmail(string Email)
        {
            var getuser = (from user in _db.UserProfile
                           where user.Email.ToLower() == Email.Trim().ToLower()
                           & user.IsDeleted == false
                           select user).ToList();

            return getuser;
        }
        public List<UserProfile> CheckDuplicateUserName(string UserName)
        {
            var getusers = (from user in _db.UserProfile
                            where user.UserName.ToLower() == UserName.Trim().ToLower()
                            && user.IsDeleted == false
                            select user).ToList();

            return getusers;
        }

        public webpages_UsersInRoles GetRoleIdByUserId(int UserId)
        {
            var getrole = (from r in _db.webpages_UsersInRoles
                           where r.UserId == UserId
                           select r).FirstOrDefault();
            return getrole;
        }
        public int GetIdByUserName(string UserName)
        {
            return _db.UserProfile.Where(x => x.UserName == UserName).FirstOrDefault().UserId;
        }
        public webpages_Membership Getwebpages_MembershipByUserId(int userId)
        {
            return _db.webpages_Membership.Where(x => x.UserId == userId).FirstOrDefault();
        }
    }
}
