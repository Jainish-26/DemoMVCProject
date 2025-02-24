using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class QuestionTypeProvider : BaseProvider
    {

        public List<QuestionType> GetAllQuestionTypes()
        {
            var data = (from s in _db.QuestionType select s).ToList();
            return data;
        }
        public IQueryable<QuestionTypesGridModel> GetAllQuestionTypesGridModel()
        {
            return (from s in _db.QuestionType
                    select new QuestionTypesGridModel
                    {
                        Id = s.QuestionTypeId,
                        Name = s.QuestionTypeName,
                        QuestionTypeCode = s.QuestionTypeCode

                    }).AsQueryable();
        }
        public int CreateQuestionType(QuestionType QType)
        {
            try
            {
                _db.QuestionType.Add(QType);
                _db.SaveChanges();
                return QType.QuestionTypeId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateQuestiontype(QuestionType QType)
        {
            try
            {
                _db.Entry(QType).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return QType.QuestionTypeId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public QuestionType GetQuestionTypeById(int id)
        {
            return _db.QuestionType.Find(id);
        }

        public QuestionType GetQuestionTypetByName(string QuestionTypeName)
        {
            return _db.QuestionType.Where(x => x.QuestionTypeName == QuestionTypeName).FirstOrDefault();
        }

        public List<QuestionType> CheckQuestionTypeCode(string QuestionTypeCode)
        {
            var getQuestionTypeDetails = (from qType in _db.QuestionType
                                     where qType.QuestionTypeCode.ToUpper().Trim() == QuestionTypeCode.ToUpper().Trim()
                                     select qType).ToList();
            return getQuestionTypeDetails;
        }

    }
}