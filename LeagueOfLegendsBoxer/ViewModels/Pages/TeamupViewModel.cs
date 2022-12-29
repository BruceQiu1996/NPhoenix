using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class TeamupViewModel : ObservableObject
    {
        private readonly Post _post;
        private readonly ITeamupService _teamupService;

        private ObservableCollection<PostResponseDto> _posts;
        public ObservableCollection<PostResponseDto> Posts
        {
            get => _posts;
            set => SetProperty(ref _posts, value);
        }

        public RelayCommand SendNewPostCommand { get; set; }
        public AsyncRelayCommand LoadedCommandAsync { get; set; }

        public TeamupViewModel(Post post, ITeamupService teamupService)
        {
            _post = post;
            _teamupService = teamupService;
            SendNewPostCommand = new RelayCommand(SendNewPost);
            LoadedCommandAsync = new AsyncRelayCommand(LoadedAsync);
        }

        private void SendNewPost()
        {
            _post.ShowDialog();
        }

        private async Task LoadedAsync()
        {
            var posts = await _teamupService.GetPostsAsync(null, null, 1);
            Posts = new ObservableCollection<PostResponseDto>(posts.Item2);
        }
    }
}
