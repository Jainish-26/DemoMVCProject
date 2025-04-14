using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class QuestionMediaService
    {
        private readonly QuestionMediaProvider _questionMediaProvider; 
        public QuestionMediaService()
        {
            _questionMediaProvider = new QuestionMediaProvider();
        }
        public void AddMedia(List<QuestionMedia> model)
        {
            _questionMediaProvider.AddMedia(model);
        }
        public List<QuestionMedia> GetMediaByQuestionId(int QuestionId)
        {
            return _questionMediaProvider.GetMediaByQuestionId(QuestionId);
        }
        public void RemoveExistingImages(List<int> DeleteIds)
        {
            _questionMediaProvider.RemoveExistingImages(DeleteIds);
        }
    }
}
