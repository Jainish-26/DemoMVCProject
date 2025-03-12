using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class QuestionService
    {
        public readonly QuestionProvider _questionProvider;

        public QuestionService()
        {
            _questionProvider = new QuestionProvider();
        }

        public List<Questions> GetAllQuestions()
        {
            return _questionProvider.GetAllQuestions();
        }

        public IQueryable<QuestionGridModel> GetAllQuestionsGridModel()
        {

            return _questionProvider.GetAllQuestionsGridModel();
        }

        public int CreateQuestion(Questions question)
        {
            return _questionProvider.CreateQuestion(question);
        }

        public Questions GetById(int id)
        {
            return _questionProvider.GetById(id);
        }
        public int UpdateQuestions(Questions Que)
        {
            return _questionProvider.UpdateQuestions(Que);
        }
        public bool DeleteQuestion(int id)
        {
            return _questionProvider.DeleteQuestion(id);
        }
        public bool DeleteQuestionImage(int id)
        {
            return _questionProvider.DeleteQuestionImage(id);
        }
        public string GetImage(int id)
        {
            return _questionProvider.GetImage(id);
        }
        public List<Questions> GetQuestionsByExamId(int examId)
        {
            return _questionProvider.GetQuestionsByExamId(examId);
        }
    }
}
