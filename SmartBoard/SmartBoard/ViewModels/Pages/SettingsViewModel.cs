using SmartBoard;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace SmartBoard.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"UiDesktopApp1 - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }
        public static double GridWidth { get; set; } = 5000;
        public static double GridHeight { get; set; } = 5000;

        public static double MinStepWindowBoard { get; set; } = 5;
        public static double MaxStepWindowBoard { get; set; } = 25;
        public static double MinScrollStep { get; set; } = 1;
        public static double MaxScrollStep { get; set; } = 5;

        public static double CurrentZoom { get; set; } = 1.0;
        public static double ZoomSpeedStep { get; set; } = 0.1;
        public static double MinZoom { get; set; } = 0.1;
        public static double MaxZoom { get; set; } = 2.0;

        public static double MinContainerWidth { get; set; } = 400;
        public static double MaxContainerWidth { get; set; } = 2000;
        public static double MinContainerHeight { get; set; } = 300;
        public static double MaxContainerHeight { get; set; } = 1500;

        public static double ScrollStep { get; set; } = 2;
        public static double ContainerWidth { get; set; } = 800;
        public static double ContainerHeight { get; set; } = 600;

        public static double ContainerX { get; set; } = GridWidth / 2;
        public static double ContainerY { get; set; } = GridHeight / 2;

        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Свойства для привязки
        public double gridWidth
        {
            get => GridWidth;
            set { GridWidth = value; OnPropertyChanged(); }
        }

        public double gridHeight
        {
            get => GridHeight;
            set { GridHeight = value; OnPropertyChanged(); }
        }

        public double minStepWindowBoard
        {
            get => MinStepWindowBoard;
            set { MinStepWindowBoard = value; OnPropertyChanged(); }
        }

        public double maxStepWindowBoard
        {
            get => maxStepWindowBoard;
            set { maxStepWindowBoard = value; OnPropertyChanged(); }
        }

        public double minScrollStep
        {
            get => MinScrollStep;
            set { MinScrollStep = value; OnPropertyChanged(); }
        }

        public double maxScrollStep
        {
            get => MaxScrollStep;
            set { MaxScrollStep = value; OnPropertyChanged(); }
        }

        public double currentZoom
        {
            get => CurrentZoom;
            set { CurrentZoom = value; OnPropertyChanged(); }
        }

        public double zoomSpeedStep
        {
            get => ZoomSpeedStep;
            set { ZoomSpeedStep = value; OnPropertyChanged(); }
        }

        public double minZoom
        {
            get => MinZoom;
            set { MinZoom = value; OnPropertyChanged(); }
        }

        public double maxZoom
        {
            get => MaxZoom;
            set { MaxZoom = value; OnPropertyChanged(); }
        }

        public double minContainerWidth
        {
            get => MinContainerWidth;
            set { MinContainerWidth = value; OnPropertyChanged(); }
        }

        public double maxContainerWidth
        {
            get => MaxContainerWidth;
            set { MaxContainerWidth = value; OnPropertyChanged(); }
        }

        public double minContainerHeight
        {
            get => MinContainerHeight;
            set { MinContainerHeight = value; OnPropertyChanged(); }
        }

        public double maxContainerHeight
        {
            get => MaxContainerHeight;
            set { MaxContainerHeight = value; OnPropertyChanged(); }
        }

        public double scrollStep
        {
            get => ScrollStep;
            set { ScrollStep = value; OnPropertyChanged(); }
        }

        public double containerWidth
        {
            get => ContainerWidth;
            set { ContainerWidth = value; OnPropertyChanged(); }
        }

        public double containerHeight
        {
            get => ContainerHeight;
            set { ContainerHeight = value; OnPropertyChanged(); }
        }
    }
}
