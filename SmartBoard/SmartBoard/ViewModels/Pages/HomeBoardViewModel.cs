using CommunityToolkit.Mvvm.ComponentModel;
using Npgsql;
using SmartBoard;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace SmartBoard.ViewModels.Pages
{
    public partial class HomeBoardViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly SettingsViewModel _settings;

        [ObservableProperty]
        private double containerWidth = 1200;

        [ObservableProperty]
        private double containerHeight = 400;

        [ObservableProperty]
        private double containerX = 0;

        [ObservableProperty]
        private double containerY = 0;

        [ObservableProperty]
        private double currnetZoom = 1.0;

        [ObservableProperty]
        private DateTime? selectedDate = DateTime.Now;

        [ObservableProperty]
        private string labelText = "Доска задач";

        public HomeBoardViewModel(SettingsViewModel settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            LabelText = SelectedDate?.ToString("dddd, d MMMM") ?? "Дата не выбрана";
        }

        partial void OnSelectedDateChanged(DateTime? value)
        {
            UpdateLabel();
        }
        public ObservableCollection<BoardColumn> Columns { get; set; } = new();

        public async Task LoadBoardAsync()
        {
            string connString = "Host=localhost;Username=postgres;Password=1;Database=postgres";
            using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            Columns.Clear();
            var columns = new List<BoardColumn>();

            var columnsCmd = new NpgsqlCommand("SELECT id, name FROM public.columns", conn);
            using var reader = await columnsCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                columns.Add(new BoardColumn
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Tasks = new ObservableCollection<Models.TaskCardModel>()
                });
            }
            reader.Close();

            var tasksCmd = new NpgsqlCommand(
               "SELECT id, title, description, type, priority, assignee, deadline, column_id FROM public.tasks",
               conn);

            using var tasksReader = await tasksCmd.ExecuteReaderAsync();
            while (await tasksReader.ReadAsync())
            {
                var task = new Models.TaskCardModel
                {
                    Id = tasksReader.GetInt32(0),
                    Title = tasksReader.GetString(1),
                    Description = tasksReader.GetString(2),
                    TaskType = tasksReader.GetString(3),
                    Priority = tasksReader.GetDouble(4),
                    Assignee = tasksReader.GetString(5),
                    Deadline = tasksReader.IsDBNull(6) ? null : tasksReader.GetDateTime(6),
                    ColumnId = tasksReader.GetInt32(7)
                };

                int columnId = tasksReader.GetInt32(7);
                var column = columns.FirstOrDefault(c => c.Id == columnId);
                column?.Tasks.Add(task);
            }

            foreach (var column in columns)
            {
                Columns.Add(column);
            }
        }
        public async Task UpdateTaskColumnAsync(int taskId, int columnId)
        {
            const string connString = "Host=localhost;Username=postgres;Password=1;Database=postgres";

            using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    "UPDATE public.tasks SET column_id = @columnId WHERE id = @taskId",
                    conn);

                cmd.Parameters.AddWithValue("columnId", columnId);
                cmd.Parameters.AddWithValue("taskId", taskId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
