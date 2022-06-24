using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string AppTitle { get; } = "MusicPlayer";

        private ViewModelBase? _currentView;
        public ViewModelBase? CurrentView
        {
            get => _currentView;
            set
            {
                if (value == _currentView) return;
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase? _playerView;
        public ViewModelBase? PlayerView
        {
            get => _playerView;
            set
            {
                if (value == _playerView) return;
                _playerView = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase? _toolbarView;
        public ViewModelBase? ToolbarView
        {
            get => _toolbarView;
            set
            {
                if (value == _toolbarView) return;
                _toolbarView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(HomeViewModel homeView, PlayerViewModel playerView, ToolbarViewModel toolbarView)
        {
            CurrentView = homeView;
            PlayerView = playerView;
            ToolbarView = toolbarView;
        }
    }
}
