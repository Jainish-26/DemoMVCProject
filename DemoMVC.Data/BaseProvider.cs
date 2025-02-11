using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
