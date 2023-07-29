using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class NewsViewModel : ObservableObject
    {
        private int _pageSize = 10;
        private readonly ITeamupService _teamupService;
        private readonly IConfiguration _configuration;

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand<Post> GoodCommandAsync { get; set; }
        public AsyncRelayCommand<Post> OpenPostDetailCommandAsync { get; set; }
        public AsyncRelayCommand<FunctionEventArgs<int>> JumpPostPageCommandAsync { get; set; }

        private ObservableCollection<Post> _topPosts;
        public ObservableCollection<Post> TopPosts
        {
            get => _topPosts;
            set => SetProperty(ref _topPosts, value);
        }

        private ObservableCollection<Post> _posts;
        public ObservableCollection<Post> Posts
        {
            get => _posts;
            set => SetProperty(ref _posts, value);
        }

        public Dictionary<string, string> PostCategories { get; set; }

        private int _page;
        public int Page
        {
            get => _page;
            set => SetProperty(ref _page, value);
        }

        private int _allPages;
        public int AllPages
        {
            get => _allPages;
            set => SetProperty(ref _allPages, value);
        }

        public NewsViewModel(ITeamupService teamupService, IConfiguration configuration, EnumHelper enumHelper)
        {
            Page = 1;
            GoodCommandAsync = new AsyncRelayCommand<Post>(GoodAsync);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            OpenPostDetailCommandAsync = new AsyncRelayCommand<Post>(OpenPostDetailAsync);
            JumpPostPageCommandAsync = new AsyncRelayCommand<FunctionEventArgs<int>>(JumpPostPageAsync);
            _teamupService = teamupService;
            _configuration = configuration;
            PostCategories = enumHelper.GetEnumItemValueDesc(typeof(PostCategory));
        }

        private async Task GoodAsync(Post post)
        {
            try
            {
                var result = await _teamupService.GoodAsync(post.Id);
                post.HadGood = result.Item1;
                post.GoodCount = result.Item2;
            }
            catch
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "服务器发生错误!",
                    ShowDateTime = false
                });
            }
        }

        public async Task OpenPostDetailAsync(Post post)
        {
            var window = App.ServiceProvider.GetRequiredService<LeagueOfLegendsBoxer.Windows.PostDetailWindow>();
            await (window.DataContext as PostDetailWindowViewModel).LoadPostDetailAsync(post);
            window.Show();
        }

        private async Task JumpPostPageAsync(FunctionEventArgs<int> e)
        {
            await LoadPostsAsync(e.Info);
        }

        private async Task LoadAsync()
        {
            Posts = new ObservableCollection<Post>();
            TopPosts = new ObservableCollection<Post>();
            var topPosts = await _teamupService.GetTopPostsAsync();
            TopPosts = new ObservableCollection<Post>(topPosts.Select(x => new Post()
            {
                Id = x.Id,
                PublisherName = x.PublisherName,
                Title = x.Title,
                Content = x.Content,
                CreateTime = x.CreateTime,
                PostCategory = x.PostCategory,
                IsDeleted = x.IsDeleted,
                IsTop = x.IsTop,
                Tag = x.Tag,
                HadGood = x.HadGood,
                GoodCount = x.GoodCount,
                PostCategoryText = PostCategories[((int)x.PostCategory).ToString()],
                PostCategoryColor = x.PostCategory switch
                {
                    PostCategory.Race => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(178, 34, 34)),
                    PostCategory.Information => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 144, 255)),
                    PostCategory.Other => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 127, 36)),
                    _ => null
                }
            }));
            await LoadPostsAsync(1);
        }

        private async Task LoadPostsAsync(int page)
        {
            var posts = await _teamupService.GetPostsAsync(null, null, page, _pageSize);
            Posts = new ObservableCollection<Post>(posts.Item2.Select(x => new Post()
            {
                Id = x.Id,
                PublisherName = x.PublisherName,
                Title = x.Title,
                Content = x.Content,
                CreateTime = x.CreateTime,
                PostCategory = x.PostCategory,
                IsDeleted = x.IsDeleted,
                IsTop = x.IsTop,
                Tag = x.Tag,
                HadGood = x.HadGood,
                GoodCount = x.GoodCount,
                PostCategoryText = PostCategories[((int)x.PostCategory).ToString()],
                PostCategoryColor = x.PostCategory switch
                {
                    PostCategory.Race => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(178, 34, 34)),
                    PostCategory.Information => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 144, 255)),
                    PostCategory.Other => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 127, 36)),
                    _ => null
                }
            }));

            AllPages = posts.Item1 / _pageSize;
            if (posts.Item1 % _pageSize != 0)
                AllPages++;
        }
    }
}
