using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;

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
        public DataTable ToDataTable<T>(List<T> items)
        {
            var dt = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dt.Rows.Add(values);
            }
            return dt;
        }
    }
}
