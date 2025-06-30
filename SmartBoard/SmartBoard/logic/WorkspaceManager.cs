using SmartBoard.Models;
using SmartBoard.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartBoard
{
    public class WorkspaceManager
    {
        private readonly FrameworkElement _mainGrid;
        private readonly ScrollViewer _scrollViewer;
        private readonly FrameworkElement _zoomContainer;
        private readonly FrameworkElement _draggableContainer;
        private readonly TextBlock _cordText;
        private readonly TextBlock _zoomText;
        private Point _dragStartPoint;
        private bool _isCardDragging;
        private TaskCardModel _draggedTask;
        private BoardColumn _sourceColumn;

        private Point _lastMousePosition;
        private bool _isDragging;

        public WorkspaceManager(
            FrameworkElement mainGrid,
            ScrollViewer scrollViewer,
            FrameworkElement zoomContainer,
            FrameworkElement draggableContainer,
            TextBlock cordText,
            TextBlock zoomText)
        {
            _mainGrid = mainGrid;
            _scrollViewer = scrollViewer;
            _zoomContainer = zoomContainer;
            _draggableContainer = draggableContainer;
            _cordText = cordText;
            _zoomText = zoomText;

            InitializeWorkspace();
        }

        public void InitializeWorkspace()
        {
            if (_mainGrid == null || _draggableContainer == null) return;

            _mainGrid.Width = SmartBoard.ViewModels.Pages.SettingsViewModel.GridWidth;
            _mainGrid.Height = SmartBoard.ViewModels.Pages.SettingsViewModel.GridHeight;

            var transform = new TranslateTransform(SmartBoard.ViewModels.Pages.SettingsViewModel.ContainerX, SmartBoard.ViewModels.Pages.SettingsViewModel.ContainerY);
            _draggableContainer.RenderTransform = transform;

            _zoomContainer.LayoutTransform = new ScaleTransform(SmartBoard.ViewModels.Pages.SettingsViewModel.CurrentZoom, SmartBoard.ViewModels.Pages.SettingsViewModel.CurrentZoom);

            _scrollViewer.ScrollToHorizontalOffset(SmartBoard.ViewModels.Pages.SettingsViewModel.ContainerX);
            _scrollViewer.ScrollToVerticalOffset(SmartBoard.ViewModels.Pages.SettingsViewModel.ContainerY);

            _zoomText.Text = $"x{SmartBoard.ViewModels.Pages.SettingsViewModel.CurrentZoom:F1}";
        }

        public void StartAreaDrag(Point position)
        {
            _isDragging = true;
            _lastMousePosition = position;
            Mouse.Capture(_mainGrid);
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        public void EndAreaDrag()
        {
            _isDragging = false;
            Mouse.Capture(null);
            Mouse.OverrideCursor = null;
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            Point position = e.GetPosition(_zoomContainer);
            _cordText.Text = $"X={position.X:F0}, Y={position.Y:F0}";
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(_scrollViewer);
                Vector offset = currentPosition - _lastMousePosition;

                _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - offset.X);
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - offset.Y);

                _lastMousePosition = currentPosition;
                e.Handled = true;
            }
        }

        public void HandleMouseWheel(MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ZoomWorkspace(e.Delta > 0 ? SmartBoard.ViewModels.Pages.SettingsViewModel.ZoomSpeedStep : -SmartBoard.ViewModels.Pages.SettingsViewModel.ZoomSpeedStep,
                              e.GetPosition(_scrollViewer));
                e.Handled = true;
            }
        }

        private void ZoomWorkspace(double zoomDelta, Point mousePos)
        {
            double oldZoom = SmartBoard.ViewModels.Pages.SettingsViewModel.CurrentZoom;
            double newZoom = oldZoom + zoomDelta;

            if (newZoom >= SmartBoard.ViewModels.Pages.SettingsViewModel.MinZoom &&
                newZoom <= SmartBoard.ViewModels.Pages.SettingsViewModel.MaxZoom)
            {
                double zoomFactor = newZoom / oldZoom;

                double offsetX = mousePos.X + _scrollViewer.HorizontalOffset;
                double offsetY = mousePos.Y + _scrollViewer.VerticalOffset;

                SmartBoard.ViewModels.Pages.SettingsViewModel.CurrentZoom = newZoom;
                _zoomContainer.LayoutTransform = new ScaleTransform(newZoom, newZoom);

                _scrollViewer.ScrollToHorizontalOffset(offsetX * zoomFactor - mousePos.X);
                _scrollViewer.ScrollToVerticalOffset(offsetY * zoomFactor - mousePos.Y);

                _zoomText.Text = $"x{newZoom:F1}";
            }
        }
        public void StartCardDrag(TaskCardModel task, Point position, BoardColumn sourceColumn)
        {
            _isCardDragging = true;
            _draggedTask = task;
            _sourceColumn = sourceColumn;
            _dragStartPoint = position;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        public void HandleCardDrag(MouseEventArgs e)
        {
            if (!_isCardDragging) return;

            Point currentPosition = e.GetPosition(_scrollViewer);
            Vector offset = currentPosition - _dragStartPoint;

        }

        public void EndCardDrag()
        {
            _isCardDragging = false;
            _draggedTask = null;
            _sourceColumn = null;
            Mouse.OverrideCursor = null;
        }
        public void HandleCardDrop(BoardColumn targetColumn)
        {
            if (!_isCardDragging || _draggedTask == null || _sourceColumn == null)
                return;

            if (_sourceColumn != targetColumn)
            {
                _sourceColumn.Tasks.Remove(_draggedTask);
                targetColumn.Tasks.Add(_draggedTask);
                _draggedTask.ColumnId = targetColumn.Id;
            }
            EndCardDrag();
        }
    }
}