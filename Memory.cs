using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accessibility;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BestCalculatorEver
{
    public interface IMemory
    {
        public IEnumerable<double> ValueList { get; }
        public void Add(double value);
        public void Remove(int index);
        public double GetValue(int index);
        public void Clear();
    }


    public class RamMemory : IMemory
    {
        public IEnumerable<double> ValueList => _valueList;

        private List<double> _valueList = new ();

        public void Add(double value)
        {
            _valueList.Add(value);
        }

        public void Remove(int index)
        {
            _valueList.RemoveAt(index);
        }

        public double GetValue(int index)
        {
            return _valueList[index];
        }

        public void Clear()
        {
            _valueList.Clear();
        }
    }

    public class FileMemory : IMemory
    {
        public IEnumerable<double> ValueList {
            get
            {
                Load();
                return _valueList;
            }
        }

        //private List<double> _valueList = new();
        private List<double> _valueList = new();

        public FileMemory()
        {
            Load();
        }

        public void Add(double value)
        {
            _valueList.Add(value);
            Save();
        }

        public void Remove(int index)
        {
            _valueList.RemoveAt(index);
            Save();
        }

        public double GetValue(int index)
        {
            return _valueList[index];
        }

        public void Clear()
        {
            _valueList.Clear();
            Save();
        }

        private void Save()
        {
            var sw = new StreamWriter("data.json");
            sw.Write(JsonSerializer.Serialize(_valueList));
            sw.Close();
        }

        private void Load()
        {
            if (!(File.Exists("data.json")))
            {
                var sw = new StreamWriter("data.json");
                sw.WriteLine("[]");
                sw.Close();
            }

            var sr = new StreamReader("data.json");
            _valueList = new(JsonSerializer.Deserialize<List<double>>(sr.ReadToEnd()));
            sr.Close();
        }
    }


    public class DataBaseMemory : IMemory
    {
        public IEnumerable<double> ValueList
        {
            get
            {
                var list = new List<double> ();
                foreach (var dbValue in db.Values.ToList())
                {
                    list.Add(dbValue.Body);
                }

                return list;

                // TODO Распаковка в лист
            }
        }
 

        private ApplicationContext db = new ();
        public DataBaseMemory()
        {
            db.Database.EnsureCreated();
            db.Values.Load();
        }

        public void Add(double value)
        {
            db.Values.Add(new DbValue() { Body = value });
            db.SaveChanges();
        }

        public void Remove(int index)
        {
            db.Values.Remove(db.Values.Find(index));
            db.SaveChanges();
        }

        public double GetValue(int index)
        {
            return db.Values.Find(index).Body;
        }

        public void Clear()
        {
            var rows = from o in db.Values select o;
            foreach (var row in rows)
            {
                db.Values.Remove(row);
            }
            db.SaveChanges();
        }
    }

    [Table("VALUES")]
    public class DbValue
    {
        public int Id { get; set; }
        public double Body { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<DbValue> Values { get; set; } = null!;

        private const string dbFileName = "helloapp.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={dbFileName}");
        }
    }
}
