using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SmartBoard.ViewModels.Pages;
using SmartBoard;

public class ContainerManager
{
    private readonly FrameworkElement _container;
    private readonly FrameworkElement _zoomContainer;
    private UIElement? _dragCaptureElement;

    private Point _dragStartPoint;
    private bool _isDraggingContainer;
    private double _remainderX;
    private double _remainderY;
    private readonly Window? _mainWindow;
    private readonly WorkspaceManager _workspaceManager;
    private readonly HomeBoardViewModel _viewModel;

    public ContainerManager(
        FrameworkElement container,
        FrameworkElement zoomContainer,
        Window mainWindow,
        WorkspaceManager workspaceManager,
        HomeBoardViewModel viewModel)
    {
        _container = container;
        _zoomContainer = zoomContainer;
        _mainWindow = mainWindow;
        _workspaceManager = workspaceManager;
        _viewModel = viewModel;
        var initialTransform = new TranslateTransform(
            SettingsViewModel.ContainerX,
            SettingsViewModel.ContainerY);

        _container.RenderTransform = initialTransform;
    }

    public void StartDragContainer(Point position, UIElement sender)
    {
        _isDraggingContainer = true;
        _dragStartPoint = position;
        _remainderX = 0;
        _remainderY = 0;
        _dragCaptureElement = sender;
        _dragCaptureElement.CaptureMouse();
    }

    public void EndDragContainer()
    {
        if (_isDraggingContainer)
        {
            _isDraggingContainer = false;
            _remainderX = 0;
            _remainderY = 0;
            _dragCaptureElement?.ReleaseMouseCapture();
            _dragCaptureElement = null;
        }
    }

    public void HandleMouseMove(MouseEventArgs e)
    {
        if (_isDraggingContainer && e.LeftButton == MouseButtonState.Pressed)
        {
            Point currentPoint = e.GetPosition(_zoomContainer);
            Vector offset = currentPoint - _dragStartPoint;

            var transform = _container.RenderTransform as TranslateTransform ?? new TranslateTransform();
            if (_container.RenderTransform == null)
                _container.RenderTransform = transform;

            double zoom = SettingsViewModel.CurrentZoom;
            double deltaX = offset.X / zoom;
            double deltaY = offset.Y / zoom;

            transform.X += deltaX;
            transform.Y += deltaY;

            SettingsViewModel.ContainerX = transform.X;
            SettingsViewModel.ContainerY = transform.Y;

            _dragStartPoint = currentPoint;
            e.Handled = true;
        }
    }

    public void HandleContainerMove(MouseEventArgs e)
    {
        if (_isDraggingContainer && e.LeftButton == MouseButtonState.Pressed && _mainWindow != null)
        {
            Point currentPoint = e.GetPosition(_mainWindow);
            Vector offset = currentPoint - _dragStartPoint;

            double step = Keyboard.IsKeyDown(Key.LeftCtrl)
                ? SettingsViewModel.MinStepWindowBoard
                : SettingsViewModel.MaxStepWindowBoard;

            _remainderX += offset.X;
            _remainderY += offset.Y;

            double moveX = Math.Round(_remainderX / step) * step;
            double moveY = Math.Round(_remainderY / step) * step;

            if (Math.Abs(moveX) >= step || Math.Abs(moveY) >= step)
            {
                var transform = _container.RenderTransform as TranslateTransform ?? new TranslateTransform();
                if (_container.RenderTransform == null)
                    _container.RenderTransform = transform;

                transform.X += moveX;
                transform.Y += moveY;

                _viewModel.ContainerX = transform.X;
                _viewModel.ContainerY = transform.Y;

                _remainderX -= moveX;
                _remainderY -= moveY;
            }

            _dragStartPoint = currentPoint;
            e.Handled = true;
            _workspaceManager.HandleMouseMove(e);
        }
    }

    public void ResizeContainer(DragDeltaEventArgs e)
    {
        double step = Keyboard.IsKeyDown(Key.LeftCtrl)
            ? SettingsViewModel.MinStepWindowBoard
            : SettingsViewModel.MaxStepWindowBoard;

        double zoom = _viewModel.CurrnetZoom > 0 ? _viewModel.CurrnetZoom : 1.0;

        double newWidth = Math.Round(_viewModel.ContainerWidth / step) * step;
        newWidth += Math.Round(e.HorizontalChange / zoom / step) * step;

        double newHeight = Math.Round(_viewModel.ContainerHeight / step) * step;
        newHeight += Math.Round(e.VerticalChange / zoom / step) * step;

        newWidth = Math.Max(SettingsViewModel.MinContainerWidth,
                      Math.Min(newWidth, SettingsViewModel.MaxContainerWidth));

        newHeight = Math.Max(SettingsViewModel.MinContainerHeight,
                           Math.Min(newHeight, SettingsViewModel.MaxContainerHeight));

        _viewModel.ContainerWidth = newWidth;
        _viewModel.ContainerHeight = newHeight;

        var transform = _container.RenderTransform as TranslateTransform;
        if (transform != null)
        {
            double maxX = Math.Max(0, _zoomContainer.ActualWidth - _container.ActualWidth);
            double maxY = Math.Max(0, _zoomContainer.ActualHeight - _container.ActualHeight);

            transform.X = Math.Max(0, Math.Min(transform.X, maxX));
            transform.Y = Math.Max(0, Math.Min(transform.Y, maxY));

            _viewModel.ContainerX = transform.X;
            _viewModel.ContainerY = transform.Y;
        }
    }
}