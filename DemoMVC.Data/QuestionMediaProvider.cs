using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DemoMVC.Data
{
    public class QuestionMediaProvider : BaseProvider
    {
        public void AddMedia (List<QuestionMedia> model)
        {
            try
            {
                _db.QuestionMedia.AddRange(model);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        public List<QuestionMedia> GetMediaByQuestionId(int QuestionId)
        {
            try
            {
                return _db.QuestionMedia
                      .Where(m => m.QuestionId == QuestionId)
                      .ToList();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void RemoveExistingImages(int QuestionId, List<string> ExistingImages)
        {
            try
            {
                var data = (from i in _db.QuestionMedia where i.QuestionId == QuestionId select i).ToList();

                foreach (var image in data)
                {
                    string fullImageName = image.MediaName + image.MediaType;

                    if (!ExistingImages.Contains(fullImageName, StringComparer.OrdinalIgnoreCase))
                    {
                        string imagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/content/QuestionImage/"), fullImageName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                    }
                    _db.QuestionMedia.Remove(image);
                }

                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }
        } 
    }
}
