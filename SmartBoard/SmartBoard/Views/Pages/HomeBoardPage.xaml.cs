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

            _cardManager = new CardManager(ViewModel);
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
            if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel task)
            {
                if (sender is Border border && border.DataContext is BoardColumn targetColumn)
                {
                    // Находим исходную колонку
                    var sourceColumn = ViewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));

                    if (sourceColumn != null && sourceColumn != targetColumn)
                    {
                        // Обновляем UI
                        sourceColumn.Tasks.Remove(task);
                        targetColumn.Tasks.Add(task);
                        task.ColumnId = targetColumn.Id;

                        // Обновляем БД
                        await ViewModel.UpdateTaskColumnAsync(task.Id, targetColumn.Id);
                    }
                }
            }
            // Обработка шаблонов задач
            else if (e.Data.GetData(typeof(TaskTemplate)) is TaskTemplate template)
            {
                if (sender is Border border && border.DataContext is BoardColumn targetColumn)
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

        private async void HandleDrop(DragEventArgs e, BoardColumn targetColumn)
        {
            if (e.Data.GetData(typeof(TaskCardModel)) is TaskCardModel task)
            {
                var sourceColumn = ViewModel.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
                if (sourceColumn != null && sourceColumn != targetColumn)
                {
                    sourceColumn.Tasks.Remove(task);
                    targetColumn.Tasks.Add(task);
                    task.ColumnId = targetColumn.Id;
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

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                    e.Data.GetDataPresent(typeof(TaskTemplate)))
                {
                    border.BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue);
                    border.BorderThickness = new Thickness(2);
                }
            }
        }

        private void Panel_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
                border.BorderThickness = new Thickness(1);
            }
        }

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TaskCardModel)) ||
                e.Data.GetDataPresent(typeof(TaskTemplate)))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
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

        private void Column_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border && border.DataContext is BoardColumn targetColumn)
            {
                _workspaceManager?.HandleCardDrop(targetColumn);
            }
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

        private async Task AddNewTask(BoardColumn column, TaskCardModel model)
        {
            using var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=1;Database=postgres");
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(
                "INSERT INTO public.tasks (title, description, type, priority, assignee, deadline, column_id) " +
                "VALUES (@title, @description, @task_type, @priority, @assignee, @deadline, @column_id) RETURNING id",
                conn);

            cmd.Parameters.AddWithValue("title", model.Title);
            cmd.Parameters.AddWithValue("description", model.Description);
            cmd.Parameters.AddWithValue("task_type", model.TaskType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("priority", model.Priority);
            cmd.Parameters.AddWithValue("assignee", model.Assignee ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("deadline", model.Deadline ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("column_id", column.Id);

            model.Id = (int)await cmd.ExecuteScalarAsync();
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


    }
}