using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class FormsProvider : BaseProvider
    {
        public FormsProvider()
        {

        }
        public int CreateForms(Forms forms)
        {
            try
            {
                _db.Forms.Add(forms);
                _db.SaveChanges();
                return forms.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateForms(Forms forms)
        {
            try
            {
                _db.Entry(forms).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return forms.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Forms GetFormsById(int id)
        {
            return _db.Forms.Find(id);
        }
        public Forms GetFormsByCode(string formcode)
        {
            return _db.Forms.Where(a => a.FormAccessCode == formcode).FirstOrDefault();
        }

        public List<Forms> GetAllForms()
        {
            var getallforms = (from f in _db.Forms
                               select new
                               {
                                   Id = f.Id,
                                   Name = ((f.ParentFormId == null ? 0 : f.ParentFormId) == 0 ? f.Name : ((from fc in _db.Forms where fc.Id == (f.ParentFormId == null ? 0 : f.ParentFormId) select fc).FirstOrDefault().Name) + " " + ">>" + " " + f.Name),
                                   NavigateURL = f.NavigateURL,
                                   DisplayOrder = f.DisplayOrder,
                                   FormAccessCode = f.FormAccessCode,
                                   IsActive = f.IsActive,
                                   IsDisplayMenu = f.IsDisplayMenu,
                                   ParentFormId = f.ParentFormId
                               }).AsEnumerable().Select(x => new Forms()
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   NavigateURL = x.NavigateURL,
                                   DisplayOrder = x.DisplayOrder,
                                   FormAccessCode = x.FormAccessCode,
                                   IsActive = x.IsActive,
                                   IsDisplayMenu = x.IsDisplayMenu,
                                   ParentFormId = x.ParentFormId
                               }).OrderByDescending(a => a.Id).ToList();

            return getallforms;
        }
        public IQueryable<FormsGridModel> GetAllFormsGrid()
        {
            return (from f in _db.Forms
                    select new
                    {
                        Id = f.Id,
                        Name = ((f.ParentFormId == null ? 0 : f.ParentFormId) == 0 ? f.Name : ((from fc in _db.Forms where fc.Id == (f.ParentFormId == null ? 0 : f.ParentFormId) select fc).FirstOrDefault().Name) + " " + ">>" + " " + f.Name),
                        NavigateURL = f.NavigateURL,
                        DisplayOrder = f.DisplayOrder,
                        FormAccessCode = f.FormAccessCode,
                        IsActive = f.IsActive,
                        IsDisplayMenu = f.IsDisplayMenu,
                        ParentFormId = f.ParentFormId
                    }).AsEnumerable().Select(x => new FormsGridModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NavigateURL = x.NavigateURL,
                        DisplayOrder = x.DisplayOrder,
                        FormAccessCode = x.FormAccessCode,
                        IsActive = x.IsActive,
                        IsDisplayMenu = x.IsDisplayMenu,
                        ParentFormId = x.ParentFormId
                    }).AsQueryable();
        }
        public List<Forms> CheckDuplicateFormAccessCode(string formAccessCode)
        {
            var getForms = (from form in _db.Forms
                            where form.FormAccessCode.ToUpper().Trim() == formAccessCode.ToUpper().Trim()
                            select form).ToList();
            return getForms;
        }
    }
}
