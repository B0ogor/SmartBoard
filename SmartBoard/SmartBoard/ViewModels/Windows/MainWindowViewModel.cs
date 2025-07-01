using SmartBoard.Models;
using SmartBoard.Views.model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace SmartBoard.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - SmartBoard";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            // === Home ===
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.HomeboardPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            },
            new NavigationViewItem()
            {
                Content = "Profile",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Person24 },
                TargetPageType = typeof(Views.Pages.ProfilePage)
            }
        };
        public ObservableCollection<TaskTemplate> TaskTemplates { get; set; }

        public MainWindowViewModel()
        {
            TaskTemplates = new ObservableCollection<TaskTemplate>
        {
            new TaskTemplate
            {
                Title = "Приложение крашится при входе пользователя",
                Description = "После ввода логина и пароля происходит crash с ошибкой NullPointerException в AuthViewModel...",
                TaskType = "Bug",
                Priority = 3.6,
                Assignee = "Иван Петров",
                Deadline = new DateTime(2025, 4, 10)
            },
            // Добавьте остальные задачи
        };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}