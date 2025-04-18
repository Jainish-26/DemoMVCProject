﻿using DemoMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace DemoMVC.Data
{
    public class QuestionProvider : BaseProvider
    {
        private readonly AnswerProvider _answerProvider;

        public QuestionProvider()
        {
            _answerProvider = new AnswerProvider();
        }
        public List<Questions> GetAllQuestions()
        {
            var data = from q in _db.Questions where q.IsActive == true select q;

            return data.ToList();
        }

        public IQueryable<QuestionGridModel> GetAllQuestionsGridModel()
        {
            return (from q in _db.Questions join
                    c in _db.CommonLookup  on q.Difficulty equals c.Code into difficultyJoin
                    from c in difficultyJoin.DefaultIfEmpty()
                    select new QuestionGridModel
                    {
                        QuestionId = q.QuestionId,
                        Type = q.QuestionType.QuestionTypeName,
                        Subject = q.Subject.SubjectName,
                        QuestionText = q.QuestionText,
                        BadgeCode = c!=null ? c.BadgeCode : null,
                        Marks = q.Marks,
                        Difficulty = q.Difficulty,
                        IsActive = q.IsActive,
                    }).AsQueryable().OrderByDescending(x => x.QuestionId);
        }


        public int CreateQuestion(Questions questiion)
        {
            try
            {
                _db.Questions.Add(questiion);
                _db.SaveChanges();
                return questiion.QuestionId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetQuestionText(int id)
        {
            return (from q in _db.Questions where q.QuestionId == id select q.QuestionText).FirstOrDefault();
        }

        public Questions GetById(int id)
        {
            return _db.Questions.Find(id);
        }

        public int UpdateQuestions(Questions Que)
        {
            try
            {
                _db.Entry(Que).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return Que.QuestionId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteQuestion(int id)
        {
            var Que = GetById(id);

            if (Que != null)
            {
                var ans = _answerProvider.GetByQuestionId(id);

                if (ans == null)
                {
                    _db.Questions.Remove(Que);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    foreach (var a in ans)
                    {
                        bool deleted = _answerProvider.DeleteAnswer(a.AnswerId);

                        if (deleted)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    _db.Questions.Remove(Que);
                    _db.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public bool DeleteQuestionImage(int id)
        {
            var question = GetAllQuestions().FirstOrDefault(q => q.QuestionId == id);

            if (question != null && !string.IsNullOrEmpty(question.QuestionImage))
            {
                // Delete image file from folder
                string imagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/content/QuestionImage/"), question.QuestionImage);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Remove image reference from database
                question.QuestionImage = null;
                _db.SaveChanges(); // Save changes to database
                return true;
            }

            return false;
        }

        public string GetImage(int id)
        {
            return (from i in _db.Questions where i.QuestionId == id select i.QuestionImage).ToString();
        }

        public List<Questions> GetQuestionsByExamId(int examId)
        {
            var questionIds = _db.ExamQuestions
                .Where(eq => eq.ExamId == examId)
                .Select(eq => eq.QuestionId)
                .ToList();

            return _db.Questions
                .Where(q => questionIds.Contains(q.QuestionId))
                .ToList();
        }

        public bool CheckQuestionInExam(int QuestionId)
        {
            int count = (from i in _db.ExamQuestions where QuestionId == i.QuestionId select i).Count();

            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable GetQuestionAndAnswerExportData()
        {
            var allData = (from q in _db.Questions
                           join a in _db.Answers on q.QuestionId equals a.QuestionId
                           join qt in _db.QuestionType on q.QuestionTypeId equals qt.QuestionTypeId
                           join s in _db.Subject on q.SubjectId equals s.SubjectId
                           select new
                           {
                               q.QuestionText,
                               q.Marks,
                               QuestionType = qt.QuestionTypeCode,
                               Subject = s.SubjectCode,
                               q.Difficulty,
                               a.AnswerText,
                               a.IsCorrect
                           }).ToList();

            var grouped = allData
                .GroupBy(q => new {q.QuestionText, q.Marks, q.QuestionType, q.Subject, q.Difficulty })
                .Select(g => new QuestionAnswerExport
                {
                    QuestionText = g.Key.QuestionText,
                    Marks = g.Key.Marks,
                    QuestionType = g.Key.QuestionType,
                    Subject = g.Key.Subject,
                    Difficulty = g.Key.Difficulty,
                    AnswerJson = JsonConvert.SerializeObject(
                        g.Select(a => new
                        {
                            a.AnswerText,
                            a.IsCorrect
                        })
                    )
                }).ToList();

            return ToDataTable(grouped);
        }

        public void CreateQuestions(List<Questions> model)
        {
            try
            {
                _db.Questions.AddRange(model);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
