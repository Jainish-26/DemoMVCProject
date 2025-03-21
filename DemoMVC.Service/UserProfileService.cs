using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class UserProfileService
    {
        public readonly UserProfileProvider _userProfileProvider;
        public UserProfileService()
        {
            _userProfileProvider = new UserProfileProvider();
        }

        public UserProfile GetUserById(int UserId)
        {
            return _userProfileProvider.GetUserById(UserId);
        }

        public UserProfile GetUserByEmailId(string emailId)
        {
            return _userProfileProvider.GetUserByEmailId(emailId);
        }

        public List<UserProfile> GetAllUserProfile(string rolecode = "")
        {
            return _userProfileProvider.GetAllUserProfile(rolecode);
        }
        public IQueryable<UserProfileGridModel> GetAllUserProfileGrid()
        {
            return _userProfileProvider.GetAllUserProfileGrid();
        }
        public int UpdateUserProfile(UserProfile userprofile)
        {
            return _userProfileProvider.UpdateUserProfile(userprofile);
        }

        public webpages_UsersInRoles GetRoleIdByUserId(int UserId)
        {
            return _userProfileProvider.GetRoleIdByUserId(UserId);
        }
        public List<UserProfile> CheckDuplicateUserName(string UserName)
        {
            return _userProfileProvider.CheckDuplicateUserName(UserName);
        }
        public List<UserProfile> CheckDuplicateUserEmail(string Email)
        {
            return _userProfileProvider.CheckDuplicateUserEmail(Email);
        }
        public int GetIdByUserName(string UserName)
        {
            return _userProfileProvider.GetIdByUserName(UserName);
        }
        public webpages_Membership Getwebpages_MembershipByUserId(int userId)
        {
            return _userProfileProvider.Getwebpages_MembershipByUserId(userId);
        }

        public List<UserStatusCountModel> IsActiveUser()
        {
            return _userProfileProvider.IsActiveUser();
        }
        public List<string> GetAllEmails()
        {
            return _userProfileProvider.GetAllEmails();
        }
    }
}
