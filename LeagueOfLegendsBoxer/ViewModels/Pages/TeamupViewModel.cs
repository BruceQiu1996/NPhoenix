using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class TeamupViewModel : ObservableObject
    {
        private bool _loaded = false;
        private int _pageSize = 10;
        private readonly Post _post;
        private readonly IConfiguration _configuration;
        private readonly ITeamupService _teamupService;

        private ObservableCollection<PostDetail> _topPosts;
        public ObservableCollection<PostDetail> TopPosts
        {
            get => _topPosts;
            set => SetProperty(ref _topPosts, value);
        }

        private ObservableCollection<PostDetail> _posts;
        public ObservableCollection<PostDetail> Posts
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

        public RelayCommand SendNewPostCommand { get; set; }
        public AsyncRelayCommand LoadedCommandAsync { get; set; }

        public TeamupViewModel(Post post,
                               EnumHelper enumHelper,
                               IConfiguration configuration,
                               ITeamupService teamupService)
        {
            Page = 1;
            _post = post;
            _teamupService = teamupService;
            _configuration = configuration;
            PostCategories = enumHelper.GetEnumItemValueDesc(typeof(PostCategory));
            SendNewPostCommand = new RelayCommand(SendNewPost);
            LoadedCommandAsync = new AsyncRelayCommand(LoadedAsync);
        }

        private void SendNewPost()
        {
            _post.ShowDialog();
        }

        private async Task LoadedAsync()
        {
            if (!_loaded)
            {
                Posts = new ObservableCollection<PostDetail>();
                TopPosts = new ObservableCollection<PostDetail>();
                var prefix = _configuration.GetSection("PostImagesPrefix").Value;
                var topPosts = await _teamupService.GetTopPostsAsync();
                var posts = await _teamupService.GetPostsAsync(null, null, Page, _pageSize);
                AllPages = posts.Item1 / _pageSize;
                if (posts.Item1 % _pageSize != 0)
                    AllPages++;

                foreach (var post in posts.Item2)
                {
                    Posts.Add(new PostDetail()
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        CreateTime = post.CreateTime,
                        PostCategory = post.PostCategory,
                        Image_1 = post.Image_1,
                        Image_2 = post.Image_2,
                        Image_3 = post.Image_3,
                        Publisher = post.Publisher,
                        UserName = post.UserName,
                        ServerArea = post.ServerArea,
                        Rank_SOLO_5x5 = post.Rank_SOLO_5x5,
                        Rank_FLEX_SR = post.Rank_FLEX_SR,
                        Desc = post.Desc,
                        GoodCount = post.GoodCount,
                        CommmentsCount = post.CommmentsCount,
                        HadGood = post.HadGood,
                        PostCategoryText = PostCategories[((int)post.PostCategory).ToString()],
                        Image_1_loc = string.IsNullOrEmpty(post.Image_1) ? null : $"{prefix}{post.Image_1}",
                        Image_2_loc = string.IsNullOrEmpty(post.Image_2) ? null : $"{prefix}{post.Image_2}",
                        Image_3_loc = string.IsNullOrEmpty(post.Image_3) ? null : $"{prefix}{post.Image_3}",
                    });
                }

                foreach (var post in topPosts)
                {
                    TopPosts.Add(new PostDetail()
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        CreateTime = post.CreateTime,
                        PostCategory = post.PostCategory,
                        Image_1 = post.Image_1,
                        Image_2 = post.Image_2,
                        Image_3 = post.Image_3,
                        Publisher = post.Publisher,
                        UserName = post.UserName,
                        ServerArea = post.ServerArea,
                        Rank_SOLO_5x5 = post.Rank_SOLO_5x5,
                        Rank_FLEX_SR = post.Rank_FLEX_SR,
                        Desc = post.Desc,
                        GoodCount = post.GoodCount,
                        CommmentsCount = post.CommmentsCount,
                        HadGood = post.HadGood,
                        PostCategoryText = PostCategories[((int)post.PostCategory).ToString()],
                        Image_1_loc = string.IsNullOrEmpty(post.Image_1) ? null : $"{prefix}{post.Image_1}",
                        Image_2_loc = string.IsNullOrEmpty(post.Image_2) ? null : $"{prefix}{post.Image_2}",
                        Image_3_loc = string.IsNullOrEmpty(post.Image_3) ? null : $"{prefix}{post.Image_3}",
                    });
                }
                _loaded = true;
            }
        }
    }
}
