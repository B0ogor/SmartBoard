using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartBoard.Models
{
    public class TaskCardModel : INotifyPropertyChanged
    {
        private int _id;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _taskType = string.Empty;
        private double _priority;
        private string _assignee = string.Empty;
        private DateTime? _deadline;
        private int _columnId;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string TaskType
        {
            get => _taskType;
            set
            {
                _taskType = value;
                OnPropertyChanged();
            }
        }

        public double Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged();
            }
        }

        public string Assignee
        {
            get => _assignee;
            set
            {
                _assignee = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged();
            }
        }

        public int ColumnId
        {
            get => _columnId;
            set
            {
                _columnId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class TaskTemplate
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TaskType { get; set; }
        public double Priority { get; set; }
        public string Assignee { get; set; }
        public DateTime? Deadline { get; set; }
    }
}