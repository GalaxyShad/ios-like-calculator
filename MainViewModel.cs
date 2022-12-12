using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MathExpressionParser;

namespace BestCalculatorEver
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _display = "";

        public string Display
        {
            get => _display;
            set
            {
                if (value == _display) return;
                _display = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<string> OnButtonClickCommand { get; }

        private readonly MathExpression _mathParser = new();

        private IMemory _memory;

        public IEnumerable<double> MemoryList => _memory.ValueList;

        public MainViewModel()
        {

            _memory = new RamMemory();

            //for (var i = 0; i < 30; i++)
            //    _memory.Add(i);

            OnButtonClickCommand = new RelayCommand<string>((action =>
            {
                switch (action)
                {
                    case "AC": 
                        Display = "";
                        break;
                    case "=":
                        DoSomeMath();
                        break;
                    case "MS":
                        _memory.Add(_mathParser.Parse(Display));
                        break;
                    default:
                        Display += action;
                        break;
                }
            }));
        }

        private void DoSomeMath()
        {
            try
            {
                Display = _mathParser.Parse(Display)
                    .ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                Display = "Ошибка";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
