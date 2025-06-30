using Npgsql;
using SmartBoard.ViewModels.Pages;
using SmartBoard.Views.model;
using System;
using System.Windows;
using System.Windows.Controls;
using SmartBoard.Models;

namespace SmartBoard
{
    public class CardManager
    {
        private readonly Panel _cardPanel;
        private readonly Panel _mainPanel;
        private readonly HomeBoardViewModel _viewModel;
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1;Database=postgres";


        public CardManager(Panel cardPanel, Panel mainPanel)
        {
            _cardPanel = cardPanel;
            _mainPanel = mainPanel;
        }

        public void AddNewCard()
        {
            var card = new TaskCard
            {
                Title = "Новая задача",
                Description = "Описание задачи",
                TaskType = "Feature",
                Priority = 5.0,
                Assignee = "Александр Смирнов",
                Deadline = DateTime.Today.AddDays(3)
            };

            _cardPanel.Children.Add(card);
        }

        public CardManager(HomeBoardViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task HandleCardDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(SmartBoard.Models.TaskCardModel)) is SmartBoard.Models.TaskCardModel task &&
                sender is Border border &&
                border.DataContext is BoardColumn targetColumn)
            {
                var sourceColumn = _viewModel.Columns
                    .FirstOrDefault(c => c.Tasks.Contains(task));

                if (sourceColumn != null)
                {
                    sourceColumn.Tasks.Remove(task);
                    targetColumn.Tasks.Add(task);
                    task.ColumnId = targetColumn.Id;

                    await UpdateTaskColumnAsync(task.Id, targetColumn.Id);
                }
            }
        }

        public async Task UpdateTaskColumnAsync(int taskId, int columnId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
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