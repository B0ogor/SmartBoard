using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SmartBoard.Models;
using SmartBoard.ViewModels.Pages;
using SmartBoard.Views.Pages;
using SmartBoard.Views.Windows;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Abstractions.Controls;

namespace SmartBoard.Views.Pages
{
    public partial class HomeboardPage : INavigableView<HomeBoardViewModel>
    {
        public HomeBoardViewModel ViewModel { get; }
        private WorkspaceManager _workspaceManager;
        private ContainerManager _containerManager;
        private Window _mainWindow;
        private CardManager _cardManager;

        public HomeboardPage(HomeBoardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
            _mainWindow = Application.Current.MainWindow;
            Loaded += async (s, e) => await ViewModel.LoadBoardAsync();

            _workspaceManager = new WorkspaceManager(
                MainGrid,
                MainScrollViewer,
                ZoomContainer,
                DraggableContainer,
                CordText,
                ZoomText);

            _containerManager = new ContainerManager(
                DraggableContainer,
                ZoomContainer,
                _mainWindow,
                _workspaceManager,
                ViewModel);
            SetupEventHandlers();

            _cardManager = new CardManager(ViewModel); // Убедитесь, что ViewModel передан
        }

        private void SetupEventHandlers()
        {
            MainGrid.MouseLeftButtonDown += OnMouseDown;
            MainGrid.MouseLeftButtonUp += OnMouseUp;
            MainGrid.MouseMove += MainGrid_MouseMove;
            MainGrid.MouseWheel += OnMouseWheel;
            DragHeader.MouseLeftButtonDown += StartDragContainer;
            DragHeader.MouseLeftButtonUp += EndDragContainer;
            DragHeader.MouseMove += DragContainer;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e) => _workspaceManager.HandleMouseWheel(e);
        
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e) => _containerManager.ResizeContainer(e);
        private void MainGrid_MouseMove(object sender, MouseEventArgs e) => _workspaceManager.HandleMouseMove(e);
        private async void Panel_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border && border.DataContext is BoardColumn targetColumn)
            {
                if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel task)
                {
                    var sourceColumn = ViewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
                    if (sourceColumn != null && sourceColumn != targetColumn)
                    {
                        sourceColumn.Tasks.Remove(task);
                        targetColumn.Tasks.Add(task);
                        await ViewModel.UpdateTaskColumnAsync(task.Id, targetColumn.Id);
                    }
                }
                else if (e.Data.GetData(typeof(TaskTemplate)) is TaskTemplate template)
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
                    await AddNewTask(targetColumn, newTask);
                }
            }
        }
        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (sender is ItemsControl itemsControl &&
                FindParentBorder(itemsControl) is Border border &&
                border.DataContext is BoardColumn targetColumn)
            {
                HandleDrop(e, targetColumn);
            }
        }

        private Border FindParentBorder(DependencyObject child)
        {
            while (child != null && !(child is Border))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as Border;
        }

        private void Panel_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
                border.BorderThickness = new Thickness(1);
            }
        }
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                    e.Data.GetDataPresent(typeof(TaskTemplate)))
                {
                    border.BorderBrush = Brushes.DeepSkyBlue;
                    border.BorderThickness = new Thickness(2);
                }
            }
        }

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                e.Data.GetDataPresent(typeof(TaskTemplate)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _workspaceManager?.HandleCardDrag(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _workspaceManager?.EndCardDrag();
        }

        private void StartDragContainer(object sender, MouseButtonEventArgs e)
        {
            _containerManager.StartDragContainer(e.GetPosition((IInputElement)_mainWindow), (UIElement)sender);
            e.Handled = true;
        }

        private void EndDragContainer(object sender, MouseButtonEventArgs e)
        {
            _containerManager.EndDragContainer();
            e.Handled = true;
        }

        private void DragContainer(object sender, MouseEventArgs e)
        {
            _containerManager.HandleContainerMove(e);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _workspaceManager.StartAreaDrag(e.GetPosition(MainScrollViewer));
            e.Handled = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _workspaceManager.EndAreaDrag();
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _workspaceManager?.InitializeWorkspace();
        }
        public async Task HandleDrop(DragEventArgs e, BoardColumn targetColumn)
        {
            if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel task)
            {
                // Перемещение существующей задачи
                var sourceColumn = ViewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
                if (sourceColumn != null && sourceColumn != targetColumn)
                {
                    sourceColumn.Tasks.Remove(task);
                    targetColumn.Tasks.Add(task);
                    await ViewModel.UpdateTaskColumnAsync(task.Id, targetColumn.Id);
                }
            }
            else if (e.Data.GetData(typeof(TaskTemplate)) is TaskTemplate template)
            {
                // Создание новой задачи из шаблона
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
                await AddNewTask(targetColumn, newTask);
            }
        }
        public async Task AddNewTask(BoardColumn column, TaskCardModel model)
        {
            const string connString = "Host=localhost;Username=postgres;Password=1;Database=postgres";

            using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            const string sql = @"
        INSERT INTO public.tasks 
            (title, description, type, priority, assignee, deadline, column_id) 
        VALUES 
            (@title, @description, @task_type, @priority, @assignee, @deadline, @column_id) 
        RETURNING id";

            using var cmd = new NpgsqlCommand(sql, conn);

            // Add parameters with proper null handling
            cmd.Parameters.AddWithValue("title", model.Title ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("description", model.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("task_type", model.TaskType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("priority", model.Priority);
            cmd.Parameters.AddWithValue("assignee", model.Assignee ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("deadline", model.Deadline.HasValue ? (object)model.Deadline.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("column_id", column.Id);

            // Execute and retrieve the new task's ID
            model.Id = (int)await cmd.ExecuteScalarAsync();

            // Update ViewModel UI collection
            model.ColumnId = column.Id;
            column.Tasks.Add(model);
        }

        private async void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var column = ViewModel.Columns.FirstOrDefault();
            if (column == null) return;

            var newTask = new TaskCardModel
            {
                Title = "Новая задача",
                Description = "Описание задачи",
                TaskType = "Feature",
                Priority = 1,
                Assignee = "Исполнитель",
                Deadline = DateTime.Now.AddDays(3)
            };

            await AddNewTask(column, newTask);
        }

        private BoardColumn _draggedFromColumn;
        private TaskCardModel _draggedTask;

        private void Column_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                    e.Data.GetDataPresent(typeof(TaskTemplate)))
                {
                    border.BorderBrush = Brushes.DeepSkyBlue;
                    border.BorderThickness = new Thickness(2);
                }
            }
        }

        private void Column_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
                border.BorderThickness = new Thickness(1);
            }
        }

        private void Column_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                e.Data.GetDataPresent(typeof(TaskTemplate)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private async void Column_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border && border.DataContext is BoardColumn targetColumn)
            {
                // Обработка перетаскивания существующей задачи
                if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel task)
                {
                    await _cardManager.HandleTaskDrag(task, targetColumn);
                }
                // Обработка создания новой задачи из шаблона
                else if (e.Data.GetData(typeof(TaskTemplate)) is TaskTemplate template)
                {
                    var newTask = await _cardManager.CreateTaskFromTemplate(template, targetColumn);
                    // Дополнительная логика при необходимости
                }

                // Сброс выделения
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
                border.BorderThickness = new Thickness(1);
            }
        }

        private void Task_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TaskCardModel task)
            {
                _draggedFromColumn = ViewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
                DragDrop.DoDragDrop(element, task, DragDropEffects.Move);
                e.Handled = true;
            }
        }

        private void Task_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

    }
}