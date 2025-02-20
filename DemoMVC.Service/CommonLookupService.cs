using DemoMVC.Data;
using DemoMVC.Helper;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class CommonLookupService
    {

        public readonly CommonLookupProvider _commonlookupProvider;
        public CommonLookupService()
        {
            _commonlookupProvider = new CommonLookupProvider();
        }

        public List<CommonLookup> GetLookupByType(string lookupType)
        {
            return _commonlookupProvider.GetLookupByType(lookupType);
        }

        public IQueryable<CommonLookUpGridModel> GetAllLookup()
        {
            return _commonlookupProvider.GetAllLookups();
        }

        public int CreateCommonLookup(CommonLookup commonlookup)
        {
            return _commonlookupProvider.CreateCommonLookup(commonlookup);
        }

        public int UpdateCommonLookup(CommonLookup commonlookup)
        {
            return _commonlookupProvider.UpdateCommonLookup(commonlookup);
        }


        public CommonLookup GetCommonLookupById(int id)
        {
            return _commonlookupProvider.GetCommonLookupById(id);
        }

        public CommonLookup GetCommonLookupByCode(string code)
        {
            return _commonlookupProvider.GetCommonLookupByCode(code);
        }
        public List<CommonLookup> CheckDuplicateLookupCode(string Code)
        {
            return _commonlookupProvider.CheckDuplicateLookupCode(Code);
        }


    }

    public static class CommonLookupHelper
    {
        public static bool EnablePersistentGridState()
        {
            CommonLookupService _lookupService = new CommonLookupService();
            CommonLookup lookupValue = _lookupService.GetLookupByType(LookupType.EnablePersistentGridState).FirstOrDefault();
            return lookupValue == null || lookupValue.Name.ToLower() != "false";
        }
    }
}
