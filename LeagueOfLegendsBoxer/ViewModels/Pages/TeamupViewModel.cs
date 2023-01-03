using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private ObservableCollection<PostBrief> _topPosts;
        public ObservableCollection<PostBrief> TopPosts
        {
            get => _topPosts;
            set => SetProperty(ref _topPosts, value);
        }

        private ObservableCollection<PostBrief> _posts;
        public ObservableCollection<PostBrief> Posts
        {
            get => _posts;
            set => SetProperty(ref _posts, value);
        }

        private ObservableCollection<ChatMessage> _chatMessages;
        public ObservableCollection<ChatMessage> ChatMessages
        {
            get => _chatMessages;
            set => SetProperty(ref _chatMessages, value);
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

        private bool _isChatPage;
        public bool IsChatPage
        {
            get => _isChatPage;
            set => SetProperty(ref _isChatPage, value);
        }

        private string _chatMessage;
        public string ChatMessage
        {
            get => _chatMessage;
            set => SetProperty(ref _chatMessage, value);
        }

        public RelayCommand SendNewPostCommand { get; set; }
        public RelayCommand OpenChatCommand { get; set; }
        public AsyncRelayCommand<PostBrief> OpenPostDetailCommandAsync { get; set; }
        public AsyncRelayCommand LoadedCommandAsync { get; set; }
        public AsyncRelayCommand<PostBrief> GoodCommandAsync { get; set; }
        public RelayCommand<string> ViewImageCommand { get; set; }
        public AsyncRelayCommand SendMessageCommandAsync { get; set; }

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
            OpenPostDetailCommandAsync = new AsyncRelayCommand<PostBrief>(OpenPostDetail);
            SendNewPostCommand = new RelayCommand(SendNewPost);
            LoadedCommandAsync = new AsyncRelayCommand(LoadedAsync);
            GoodCommandAsync = new AsyncRelayCommand<PostBrief>(GoodAsync);
            ViewImageCommand = new RelayCommand<string>(ViewImage);
            OpenChatCommand = new RelayCommand(OpenChat);
            SendMessageCommandAsync = new AsyncRelayCommand(SendMessageAsync);
            ChatMessages = new ObservableCollection<ChatMessage>();
        }

        private void SendNewPost()
        {
            _post.ShowDialog();
        }

        private void OpenChat()
        {
            if (App.HubConnection == null || App.HubConnection.State != HubConnectionState.Connected)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "未连接上世界聊天频道!请稍后再试或重启软件",
                    ShowDateTime = false
                });

                return;
            }

            IsChatPage = true;
        }

        private void ViewImage(string imageLoc) 
        {
            if (string.IsNullOrEmpty(imageLoc))
                return;

            new ImageBrowser(new Uri(imageLoc)).Show();
        }

        private async Task LoadedAsync()
        {
            if (!_loaded)
            {
                Posts = new ObservableCollection<PostBrief>();
                TopPosts = new ObservableCollection<PostBrief>();
                var prefix = _configuration.GetSection("PostImagesPrefix").Value;
                var topPosts = await _teamupService.GetTopPostsAsync();
                var posts = await _teamupService.GetPostsAsync(null, null, Page, _pageSize);
                AllPages = posts.Item1 / _pageSize;
                if (posts.Item1 % _pageSize != 0)
                    AllPages++;

                foreach (var post in posts.Item2)
                {
                    Posts.Add(new PostBrief()
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
                    TopPosts.Add(new PostBrief()
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

        private async Task GoodAsync(PostBrief postDetail) 
        {
            try
            {
                var result = await _teamupService.GoodAsync(postDetail.Id);
                var p1 = Posts.FirstOrDefault(x => x == postDetail);
                if (p1 != null)
                {
                    p1.HadGood = result.Item1;
                    p1.GoodCount = result.Item2;
                }

                var p2 = TopPosts.FirstOrDefault(x => x == postDetail);
                if (p2 != null)
                {
                    p2.HadGood = result.Item1;
                    p2.GoodCount = result.Item2;
                }
            }
            catch (Exception ex) 
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "服务器发生错误!",
                    ShowDateTime = false
                });
            }
        }

        private async Task OpenPostDetail(PostBrief postDetail) 
        {
            var detail = App.ServiceProvider.GetRequiredService<PostDetailWindow>();
            await (detail.DataContext as PostDetailWindowViewModel).LoadPostDetailAsync(postDetail.Id);
            detail.Show();
        }

        private async Task SendMessageAsync() 
        {
            if (string.IsNullOrEmpty(ChatMessage) || ChatMessage.Length > 50) 
            {
                return;
            }

            await App.HubConnection.SendAsync("SendMessage", ChatMessage);
            ChatMessage = null;
        }
    }
}
