using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class SubjectService
    {
        private readonly SubjectProvider _subjectProvider;
        public SubjectService()
        {
            _subjectProvider = new SubjectProvider();
        }

        public List<Subject> GetAllSubjects()
        {
            return _subjectProvider.GetAllSubjects();
        }
        public IQueryable<SubjectGridModel> GetSubjectGridModels()
        {
            return _subjectProvider.GetAllSubjectsGrid();
        }

        public int CreateSubjects(Subject subject)
        {
            return _subjectProvider.CreateSubjects(subject);
        }
        public int UpdateSubject(Subject subject)
        {
            return _subjectProvider.UpdateSubject(subject);
        }

        public Subject GetSubjectById(int id)
        {
            return _subjectProvider.GetSubjectById(id);
        }

        public Subject GetSubjectByName(string subjectName)
        {
            return _subjectProvider.GetSubjectByName(subjectName);
        }

        public List<Subject> CheckDuplicateSubjectCode(string subjectCode)
        {
            return _subjectProvider.CheckDuplicateSubjectCode(subjectCode);
        }
    }
}
