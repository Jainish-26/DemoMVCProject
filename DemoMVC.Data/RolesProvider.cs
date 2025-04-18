using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class RolesProvider : BaseProvider
    {
        private webpages_UsersInRoles _userInRole;
        public RolesProvider()
        {
            _userInRole = new webpages_UsersInRoles();
        }
        public List<webpages_Roles> GetAllRoles()
        {
            var data = (from a in _db.webpages_Roles where a.IsActive == true && a.IsDeleted != true select a).OrderByDescending(a => a.RoleId).ToList();
            return data;
        }
        public IQueryable<RolesGridModel> GetAllRolesGrid()
        {
            return (from role in _db.webpages_Roles
                    where role.IsDeleted != true
                    select new RolesGridModel()
                    {
                        Id = role.RoleId,
                        IsActive = role.IsActive,
                        Name = role.RoleName,
                        RoleCode = role.RoleCode
                    }).AsQueryable();
        }
        public int CreateRoles(webpages_Roles role)
        {
            try
            {
                _db.webpages_Roles.Add(role);
                _db.SaveChanges();
                return role.RoleId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateRoles(webpages_Roles role)
        {
            try
            {
                _db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return role.RoleId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public webpages_Roles GetRolesById(int id)
        {
            return _db.webpages_Roles.Find(id);
        }

        public webpages_Roles GetRolesByName(string roleName)
        {
            return _db.webpages_Roles.Where(x => x.RoleName == roleName).FirstOrDefault();
        }
        public List<webpages_Roles> CheckDuplicateRoleCode(string RoleCode)
        {
            var getRoleDetails = (from role in _db.webpages_Roles
                                  where role.RoleCode.ToUpper().Trim() == RoleCode.ToUpper().Trim()
                                  select role).ToList();
            return getRoleDetails;
        }

        public bool DeleteRole(int id, int userId)
        {
            var role = GetRolesById(id);


            var userInRoleIds = _db.webpages_UsersInRoles
                      .Where(u => u.RoleId == id)
                      .Select(u => u.UserId)
                      .ToList();

            if (userInRoleIds.Any())
            {
                return false;
            }

            _db.webpages_Roles.Remove(role);
            _db.Entry(role).State = System.Data.Entity.EntityState.Deleted;
            _db.SaveChanges();

            return true;
        }

        public List<RoleUserCountModel> GetRolesWithUserCount()
        {
            var data = _db.webpages_Roles.Where(x=>x.RoleCode != "SADMIN")
            .GroupJoin(
                _db.webpages_UsersInRoles,
                role => role.RoleId,
                userInRole => userInRole.RoleId,
                (role, users) => new RoleUserCountModel
                {
                    RoleName = role.RoleName,
                    IsActive = role.IsActive,
                    UserCount = users.Count()
                }
            ).ToList();
            return data;
        }

    }
}
