using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class RoleService
    {
        public readonly RolesProvider _roleProvider;
        public RoleService()
        {
            _roleProvider = new RolesProvider();
        }

        public List<webpages_Roles> GetAllRoles()
        {
            return _roleProvider.GetAllRoles();
        }
        public IQueryable<RolesGridModel> GetAllRolesGrid()
        {
            return _roleProvider.GetAllRolesGrid();
        }

        public int CreateRoles(webpages_Roles role)
        {
            return _roleProvider.CreateRoles(role);
        }

        public int UpdateRoles(webpages_Roles role)
        {
            return _roleProvider.UpdateRoles(role);
        }

        public webpages_Roles GetRolesById(int id)
        {
            return _roleProvider.GetRolesById(id);
        }

        public webpages_Roles GetRolesByName(string roleName)
        {
            return _roleProvider.GetRolesByName(roleName);
        }
        public List<webpages_Roles> CheckDuplicateRoleCode(string RoleCode)
        {
            return _roleProvider.CheckDuplicateRoleCode(RoleCode);
        }
    }
}
