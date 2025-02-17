using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class FormRoleMappingService
    {
        public readonly FormRoleMappingProvider _formrolemappingProvider;
        public FormRoleMappingService()
        {
            _formrolemappingProvider = new FormRoleMappingProvider();
        }


        public List<FormRoleMapping> GetAllRoleRights()
        {
            return _formrolemappingProvider.GetAllRoleRights();
        }

        public int InsertRoleRights(FormRoleMapping rolerights)
        {
            return _formrolemappingProvider.InsertRoleRights(rolerights);
        }

        public List<FormRoleMapping> GetAllRoleRightsById(int RoleId)
        {
            return _formrolemappingProvider.GetAllRoleRightsById(RoleId);
        }

        public bool UpdateRoleRights(IEnumerable<FormRoleMapping> rolerights, int CreatedBy, int UpdatedBy)
        {

            return _formrolemappingProvider.UpdateRoleRights(rolerights, CreatedBy, UpdatedBy);
        }
        public List<FormRoleMapping> GetAllRoleRightsByRoleId(int RoleId)
        {
            return _formrolemappingProvider.GetAllRoleRightsByRoleId(RoleId);
        }

        public List<MenuVW> GetMenu(int userID)
        {
            return _formrolemappingProvider.GetMenu(userID);
        }

        public List<MenuVW> BindNLevelMenu(List<MenuVW> _subMenu, List<MenuVW> _fullMenu)
        {
            return _formrolemappingProvider.BindNLevelMenu(_subMenu, _fullMenu);
        }

        public FormRoleMapping CheckFormAccess(string _formaccessCode, int _roleID)
        {
            return _formrolemappingProvider.CheckFormAccess(_formaccessCode, _roleID);
        }
    }
}
