using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accessibility;

namespace BestCalculatorEver
{
    public interface IMemory
    {
        public IEnumerable<double> ValueList { get; }
        public int Count { get; }
        public void Add(double value);
        public void Remove(int index);
        public double GetValue(int index);
        public void Clear();
    }


    public class RamMemory : IMemory
    {
        public IEnumerable<double> ValueList => _valueList;
        public int Count => _valueList.Count;

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
}
