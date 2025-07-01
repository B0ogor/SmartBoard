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
        private readonly HomeBoardViewModel _viewModel;
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1;Database=postgres";

        public CardManager(HomeBoardViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task HandleTaskDrag(TaskCardModel task, BoardColumn targetColumn)
        {
            if (task == null || targetColumn == null) return;

            var sourceColumn = _viewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));

            if (sourceColumn != null && sourceColumn != targetColumn)
            {
                // Удаление из исходной колонки
                sourceColumn.Tasks.Remove(task);

                // Добавление в целевую колонку
                targetColumn.Tasks.Add(task);
                task.ColumnId = targetColumn.Id;

                // Обновление в БД
                await UpdateTaskColumnAsync(task.Id, targetColumn.Id);
            }
        }

        public async Task<TaskCardModel> CreateTaskFromTemplate(TaskTemplate template, BoardColumn targetColumn)
        {
            var newTask = new TaskCardModel
            {
                Title = template.Title,
                Description = template.Description,
                TaskType = template.TaskType,
                Priority = template.Priority,
                Assignee = template.Assignee,
                Deadline = template.Deadline,
                ColumnId = targetColumn.Id
            };

            await AddNewTaskToDatabase(newTask, targetColumn);
            return newTask;
        }

        private async Task AddNewTaskToDatabase(TaskCardModel task, BoardColumn column)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
        INSERT INTO public.tasks 
            (title, description, type, priority, assignee, deadline, column_id) 
        VALUES 
            (@title, @description, @task_type, @priority, @assignee, @deadline, @column_id) 
        RETURNING id";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("title", (object)task.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("description", (object)task.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("task_type", (object)task.TaskType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("priority", task.Priority);
            cmd.Parameters.AddWithValue("assignee", (object)task.Assignee ?? DBNull.Value);
            cmd.Parameters.AddWithValue("deadline", task.Deadline.HasValue
                ? (object)task.Deadline.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("column_id", column.Id);

            task.Id = (int)await cmd.ExecuteScalarAsync();
            column.Tasks.Add(task);
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