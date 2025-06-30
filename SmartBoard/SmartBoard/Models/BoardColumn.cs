using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBoard.Models;
public class BoardColumn
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<TaskCardModel> Tasks { get; set; } = new();
}
