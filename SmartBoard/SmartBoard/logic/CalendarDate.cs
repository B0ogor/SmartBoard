using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoard
{
    public class CalendarDate : INotifyPropertyChanged
    {
        private DateTime? _selectedDate = DateTime.Today;
        private string _labelText = string.Empty;

        public CalendarDate()
        {
            UpdateLabel();
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                UpdateLabel();
                OnPropertyChanged();
            }
        }

        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                OnPropertyChanged();
            }
        }

        private void UpdateLabel()
        {
            LabelText = _selectedDate?.ToString("dddd, d MMMM") ?? "Дата не выбрана";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}