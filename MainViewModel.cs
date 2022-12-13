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
        private string _display = string.Empty;

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

        public string CurrentMemoryMode => _memory.GetType().Name;

        public RelayCommand<string> OnButtonClickCommand { get; }
        public RelayCommand<int> OnRemoveItem { get; }
        public RelayCommand<int> OnGetItem { get; }
        public RelayCommand OnChangeMemoryMode { get; }

        private readonly MathExpression _mathParser = new();

        private IMemory _memory;

        private readonly IMemory[] _memoryList =
        {
            new RamMemory(),
            new DataBaseMemory(),
            new FileMemory()
        };

        private int _curMemory = 0;

        public ObservableCollection<Value> MemoryList => new (_memory.ValueList);

        public MainViewModel()
        {
            _memory = _memoryList[_curMemory];

            OnButtonClickCommand = new RelayCommand<string>(OnOnButtonClickCommandHandle);
            OnRemoveItem = new RelayCommand<int>(OnRemoveItemHandle);
            OnGetItem = new RelayCommand<int>(ExtractFromMemory);
            OnChangeMemoryMode = new RelayCommand(() =>
            {
                _curMemory++;
                if (_curMemory > 2) _curMemory = 0;
                _memory = _memoryList[_curMemory];

                OnPropertyChanged(nameof(CurrentMemoryMode));
                OnPropertyChanged(nameof(MemoryList));
            });
        }

        private void OnOnButtonClickCommandHandle(string? action)
        {
            switch (action)
            {
                case "AC":
                    Display = string.Empty;
                    break;
                case "=":
                    DoSomeMath();
                    break;
                case "MS":
                    SendToMemory();
                    break;
                case "MC":
                    ClearMemory();
                    break;
                default:
                    Display += action;
                    break;
            }
        }

        private void SendToMemory()
        {
            try
            {
                _memory.Add(double.Parse(Display, CultureInfo.InvariantCulture));
                OnPropertyChanged(nameof(MemoryList));
                Display = string.Empty;
            }
            catch
            {
                Display = "Ошибка";
            }
        }

        private void ExtractFromMemory(int id)
        {
            Display += _memory.GetValue(id).ToString(CultureInfo.InvariantCulture);
        }

        private void ClearMemory()
        {
            _memory.Clear();
            OnPropertyChanged(nameof(MemoryList));
        }

        private void OnRemoveItemHandle(int id)
        {
            _memory.Remove(id);
            OnPropertyChanged(nameof(MemoryList));
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
