using MusicPlayerClient.Interfaces;
using MusicPlayerClient.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IStore _mediaStore;
        private readonly IStore _playlistStore;
        public string AppTitle { get; } = "MusicPlayer";

        private ViewModelBase? _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView!;
            set
            {
                if (value == _currentView) return;
                _currentView?.Dispose();
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase? _playerView;
        public ViewModelBase PlayerView
        {
            get => _playerView!;
            set
            {
                if (value == _playerView) return;
                _playerView?.Dispose();
                _playerView = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase? _toolbarView;
        public ViewModelBase ToolbarView
        {
            get => _toolbarView!;
            set
            {
                if (value == _toolbarView) return;
                _toolbarView?.Dispose();
                _toolbarView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(HomeViewModel homeView, PlayerViewModel playerView, ToolbarViewModel toolbarView, MediaStore mediaStore, PlaylistStore playlistStore)
        {
            CurrentView = homeView;
            PlayerView = playerView;
            ToolbarView = toolbarView;
            _mediaStore = mediaStore;
            _playlistStore = playlistStore;
        }

        public override async Task InitViewModel()
        {
            await Task.WhenAll(
                _mediaStore.LoadStore(),
                _playlistStore.LoadStore()
            );

            await Task.WhenAll(
                CurrentView.InitViewModel(),
                PlayerView.InitViewModel(),
                ToolbarView.InitViewModel()
            );
        }
    }
}