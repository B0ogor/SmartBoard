using SmartBoard.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
public class BoardColumn : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    private ObservableCollection<TaskCardModel> _tasks = new();
    public ObservableCollection<TaskCardModel> Tasks
    {
        get => _tasks;
        set
        {
            _tasks = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
