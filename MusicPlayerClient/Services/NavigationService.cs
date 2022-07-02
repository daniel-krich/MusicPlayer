using MusicPlayerClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Services
{
    public interface INavigationService
    {
        public void NavigateHome();
        public void NavigatePlaylist();
        public void NavigateDownloads();
    }

    public class NavigationService: INavigationService
    {
        private readonly Func<MainViewModel>? _mainViewModelFunc;
        private readonly Func<HomeViewModel>? _homeViewModelFunc;
        private readonly Func<PlaylistViewModel>? _playlistViewModelFunc;
        private readonly Func<DownloadsViewModel>? _downloadViewModelFunc;

        public NavigationService(Func<MainViewModel> mainViewModelFunc, Func<HomeViewModel> homeViewModelFunc,
                                 Func<PlaylistViewModel> playlistViewModelFunc, Func<DownloadsViewModel> downloadViewModelFunc)
        {
            _mainViewModelFunc = mainViewModelFunc;
            _homeViewModelFunc = homeViewModelFunc;
            _playlistViewModelFunc = playlistViewModelFunc;
            _downloadViewModelFunc = downloadViewModelFunc;
        }

        public void NavigateHome()
        {
            var mainVm = _mainViewModelFunc?.Invoke();
            var homeVm = _homeViewModelFunc?.Invoke();

            if (mainVm != null && mainVm.CurrentView is not HomeViewModel)
            {
                mainVm.CurrentView = homeVm;
            }
        }

        public void NavigatePlaylist()
        {
            var mainVm = _mainViewModelFunc?.Invoke();
            var playlistVm = _playlistViewModelFunc?.Invoke();

            if (mainVm != null)
            {
                mainVm.CurrentView = playlistVm;
            }
        }

        public void NavigateDownloads()
        {
            var mainVm = _mainViewModelFunc?.Invoke();
            var downloadsVm = _downloadViewModelFunc?.Invoke();

            if (mainVm != null && mainVm.CurrentView is not DownloadsViewModel)
            {
                mainVm.CurrentView = downloadsVm;
            }
        }
    }
}
