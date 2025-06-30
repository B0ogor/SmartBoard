using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace SmartBoard.ViewModels.Pages
{
    public partial class ProfileViewModel : ObservableObject
    {
        private string _username = "Загрузка...";
        private int _level = 0;
        private int _totalTasks = 0;
        private int _completedTasks = 0;
        private double _completionPercentage = 0.0;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public int TotalTasks
        {
            get => _totalTasks;
            set => SetProperty(ref _totalTasks, value);
        }

        public int CompletedTasks
        {
            get => _completedTasks;
            set => SetProperty(ref _completedTasks, value);
        }

        public double CompletionPercentage
        {
            get => _completionPercentage;
            set => SetProperty(ref _completionPercentage, value);
        }

        public async Task LoadProfileDataAsync()
        {
            Username = "Ошибка загрузки";
        }
    }
}