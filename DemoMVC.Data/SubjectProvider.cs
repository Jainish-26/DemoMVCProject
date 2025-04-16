using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class SubjectProvider : BaseProvider
    {

        public List<Subject> GetAllSubjects()
        {
            var data = (from s in _db.Subject where s.IsActive == true select s).ToList();
            return data;
        }
        public IQueryable<SubjectGridModel> GetAllSubjectsGrid()
        {
            return (from s in _db.Subject
                    select new SubjectGridModel
                    {
                        Id = s.SubjectId,
                        IsActive = s.IsActive,
                        Name = s.SubjectName,
                        SubjectCode = s.SubjectCode

                    }).AsQueryable();
        }
        public int CreateSubjects(Subject subject)
        {
            try
            {
                _db.Subject.Add(subject);
                _db.SaveChanges();
                return subject.SubjectId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateSubject(Subject subject)
        {
            try
            {
                _db.Entry(subject).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return subject.SubjectId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Subject GetSubjectById(int id)
        {
            return _db.Subject.Find(id);
        }

        public Subject GetSubjectByName(string subjectName)
        {
            return _db.Subject.Where(x => x.SubjectName == subjectName).FirstOrDefault();
        }

        public Subject GetSubjectByCode(string subjectCode)
        {
            return _db.Subject.Where(x => x.SubjectCode == subjectCode).FirstOrDefault();
        }

        public List<Subject> CheckDuplicateSubjectCode(string subjectCode)
        {
            var getsubjectDetails = (from subject in _db.Subject
                                     where subject.SubjectCode.ToUpper().Trim() == subjectCode.ToUpper().Trim()
                                     select subject).ToList();
            return getsubjectDetails;
        }

    }
}