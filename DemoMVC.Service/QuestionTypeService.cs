using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoMVC.Service
{
    public class QuestionTypeService
    {
        private readonly QuestionTypeProvider _questionTypeProvider;
        public QuestionTypeService()
        {
            _questionTypeProvider = new QuestionTypeProvider();
        }

        public List<QuestionType> GetAllQuestionTypes()
        {
            return _questionTypeProvider.GetAllQuestionTypes();
        }
        public IQueryable<QuestionTypesGridModel> GetAllQuestionTypesGridModels()
        {
            return _questionTypeProvider.GetAllQuestionTypesGridModel();
        }

        public int CreateQuestionType(QuestionType qType)
        {
            return _questionTypeProvider.CreateQuestionType(qType);
        }
        public int UpdateQuestionType(QuestionType qType)
        {
            return _questionTypeProvider.UpdateQuestiontype(qType);
        }

        public QuestionType GetQuestionTypeById(int id)
        {
            return _questionTypeProvider.GetQuestionTypeById(id);
        }

        public QuestionType GetQuestionTypeByName(string qTypeName)
        {
            return _questionTypeProvider.GetQuestionTypetByName(qTypeName);
        }

        public List<QuestionType> CheckDuplicateQuestionTypeCode(string qTypeCode)
        {
            return _questionTypeProvider.CheckQuestionTypeCode(qTypeCode);
        }
        public DataTable GetQuestionType()
        {
            return _questionTypeProvider.GetQuestionType();
        }
        public QuestionType GetQuestionTypetByCode(string QuestionTypeCode)
        {
            return _questionTypeProvider.GetQuestionTypetByCode(QuestionTypeCode);
        }
    }
}
