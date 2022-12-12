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
        public RelayCommand<string> OnRemoveItem { get; }

        private readonly MathExpression _mathParser = new();

        private readonly IMemory _memory;

        public ObservableCollection<double> MemoryList => new (_memory.ValueList);

        public MainViewModel()
        {

            //_memory = new RamMemory();
            _memory = new DataBaseMemory();
            //_memory = new FileMemory();

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
                        OnPropertyChanged(nameof(MemoryList));
                        break;
                    case "MC":
                        _memory.Clear();
                        OnPropertyChanged(nameof(MemoryList));
                        break;
                    default:
                        Display += action;
                        break;
                }
            }));

            //OnRemoveItem = new RelayCommand<int>((element) =>
            //{
                //_memory.Remove(_memory.ValueList.);
                //OnPropertyChanged(nameof(MemoryList));
            //});
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
