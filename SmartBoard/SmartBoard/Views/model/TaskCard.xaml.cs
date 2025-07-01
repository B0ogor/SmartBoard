using SmartBoard.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartBoard.Views.model
{
    public partial class TaskCard : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TaskCard),
            new PropertyMetadata("Задача"));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(TaskCard),
            new PropertyMetadata("Описание задачи"));

        public static readonly DependencyProperty TaskTypeProperty =
            DependencyProperty.Register("TaskType", typeof(string), typeof(TaskCard),
            new PropertyMetadata("Bug"));

        public static readonly DependencyProperty PriorityProperty =
            DependencyProperty.Register("Priority", typeof(double), typeof(TaskCard),
            new PropertyMetadata(5.0));

        public static readonly DependencyProperty AssigneeProperty =
            DependencyProperty.Register("Assignee", typeof(string), typeof(TaskCard),
            new PropertyMetadata("Не назначен"));

        public static readonly DependencyProperty DeadlineProperty =
            DependencyProperty.Register("Deadline", typeof(DateTime?), typeof(TaskCard),
            new PropertyMetadata(null));

        private bool _isDragging = false;
        private Point? _dragStartPoint = null;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string TaskType
        {
            get => (string)GetValue(TaskTypeProperty);
            set => SetValue(TaskTypeProperty, value);
        }

        public double Priority
        {
            get => (double)GetValue(PriorityProperty);
            set => SetValue(PriorityProperty, value);
        }

        public string Assignee
        {
            get => (string)GetValue(AssigneeProperty);
            set => SetValue(AssigneeProperty, value);
        }

        public DateTime? Deadline
        {
            get => (DateTime?)GetValue(DeadlineProperty);
            set => SetValue(DeadlineProperty, value);
        }

        public TaskCard()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            this.MouseLeftButtonUp += OnMouseLeftButtonUp;
            this.LostMouseCapture += OnLostMouseCapture;
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;

            this.MouseMove += OnMouseMove;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                this.ReleaseMouseCapture();
            }
            e.Handled = true;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        public void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Hand);
            }
            else
            {
                e.UseDefaultCursors = true;
            }
            e.Handled = true;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.85;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.Opacity = 1.0;
        }

        public void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _dragStartPoint = e.GetPosition(this);
            }
            e.Handled = false;
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _dragStartPoint.HasValue && !_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                Vector diff = currentPosition - _dragStartPoint.Value;
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;
                    var model = this.DataContext as SmartBoard.Models.TaskCardModel;
                    if (model != null)
                    {
                        System.Windows.MessageBox.Show($"Drag started: {model.Title}");
                        DragDrop.DoDragDrop(this, model, DragDropEffects.Move);
                    }
                    _isDragging = false;
                    _dragStartPoint = null;
                }
            }
        }
        private WorkspaceManager _workspaceManager;

        public void SetWorkspaceManager(WorkspaceManager manager)
        {
            _workspaceManager = manager;
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _dragStartPoint = e.GetPosition(this);
            }
            e.Handled = false;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                _dragStartPoint.HasValue &&
                !_isDragging &&
                this.DataContext is TaskCardModel model)
            {
                Point currentPosition = e.GetPosition(this);
                Vector diff = currentPosition - _dragStartPoint.Value;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;
                    DragDrop.DoDragDrop(this, model, DragDropEffects.Move);
                    _isDragging = false;
                    _dragStartPoint = null;
                }
            }
        }
    }
}