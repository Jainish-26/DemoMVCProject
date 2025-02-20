using DemoMVC.Models;
using System;

namespace DemoMVC.Data
{
    public class BaseProvider : IDisposable
    {
        public DemoMVCEntities _db;
        public BaseProvider()
        {
            _db = new DemoMVCEntities();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
