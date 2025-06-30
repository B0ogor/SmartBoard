using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace SmartBoard.Models
{
    public class TaskCardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TaskType { get; set; }
        public double Priority { get; set; }
        public string Assignee { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
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