using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class CommonLookupProvider : BaseProvider
    {
        public CommonLookupProvider()
        {

        }

        public List<CommonLookup> GetLookupByType(string lookupType)
        {
            var data = (from c in _db.CommonLookup
                        where c.Type.Trim().ToLower() == lookupType.Trim().ToLower() && c.IsActive == true
                        select c
                        ).OrderBy(c => c.DisplayOrder).ToList();
            return data;
        }

        public IQueryable<CommonLookUpGridModel> GetAllLookups(bool isActive = false)
        {
            return (from c in _db.CommonLookup
                    select new CommonLookUpGridModel()
                    {
                        Code = c.Code.ToUpper(),
                        Comment = c.Comment,
                        DisplayOrder = c.DisplayOrder,
                        Id = c.Id,
                        IsActive = c.IsActive,
                        Name = c.Name,
                        Type = c.Type
                    });
        }

        public int CreateCommonLookup(CommonLookup commonlookup)
        {
            try
            {
                _db.CommonLookup.Add(commonlookup);
                _db.SaveChanges();
                return commonlookup.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCommonLookup(CommonLookup commonlookup)
        {
            try
            {
                _db.Entry(commonlookup).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return commonlookup.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CommonLookup GetCommonLookupById(int id)
        {
            return _db.CommonLookup.Find(id);
        }

        public CommonLookup GetCommonLookupByCode(string code)
        {
            return _db.CommonLookup.Where(x => x.Code == code && x.IsActive).FirstOrDefault();
        }
        public List<CommonLookup> CheckDuplicateLookupCode(string Code)
        {
            var getLookupDetails = (from commonLookup in _db.CommonLookup
                                    where commonLookup.Code.ToUpper().Trim() == Code.ToUpper().Trim()
                                    select commonLookup).ToList();
            return getLookupDetails;
        }
    }
}
