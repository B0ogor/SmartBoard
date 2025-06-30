using SmartBoard.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace SmartBoard.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : INavigableView<ProfileViewModel>
    {
        public ProfileViewModel ViewModel { get; }

        public ProfilePage(ProfileViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel; // Установите DataContext на ViewModel
            InitializeComponent();

            // Подписка на событие загрузки страницы
            Loaded += async (sender, e) => await ViewModel.LoadProfileDataAsync();
        }
    }
}
