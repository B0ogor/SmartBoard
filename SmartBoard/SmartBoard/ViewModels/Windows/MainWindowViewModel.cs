using SmartBoard.Views.model;
using System.Collections.ObjectModel;
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
    }
}