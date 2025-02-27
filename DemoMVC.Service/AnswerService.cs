using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class AnswerService
    {
        public readonly AnswerProvider _answerProvider;

        public AnswerService()
        {
            _answerProvider = new AnswerProvider();
        }

        public List<Answers> GetAllAnswers()
        {
            return _answerProvider.GetAllAnswers();
        }

        public int CreateAnswer(Answers ans)
        {
            return _answerProvider.CreateAnswer(ans);
        }

        public Answers GetById(int id)
        {
            return _answerProvider.GetById(id);
        }

        public List<Answers> GetByQuestionId(int id)
        {
            return _answerProvider.GetByQuestionId(id);
        }

        public int UpdateAnswers(Answers ans)
        {
            return _answerProvider.UpdateAnswer(ans);
        }

        public bool DeleteAnswer(int id)
        {
            return _answerProvider.DeleteAnswer(id);
        }
    }
}
