using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Service
{
    public class ExamLinkService
    {
        private readonly ExamLinkProvider _examLinkProvider;

        public ExamLinkService()
        {
            _examLinkProvider = new ExamLinkProvider();
        }
        public ExamLinks GetExamLinkByExamId(int ExamId)
        {
            return _examLinkProvider.GetExamLinkByExamId(ExamId);
        }
        public int CreateExamLink(ExamLinks link)
        {
            return _examLinkProvider.CreateExamLink(link);
        }
        public int UpdateExamLink(ExamLinks link)
        {
            return _examLinkProvider.UpdateExamLink(link);
        }
    }
}
