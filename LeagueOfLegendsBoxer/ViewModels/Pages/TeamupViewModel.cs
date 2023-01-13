using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpVectors.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class TeamupViewModel : ObservableObject
    {
        private bool _loaded = false;
        private int _pageSize = 10;
        private readonly Post _post;
        private readonly IConfiguration _configuration;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ITeamupService _teamupService;
        private DateTime? _lastSendTime;

        public System.Windows.Controls.ScrollViewer ChatScrollViewer { get; set; }

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

        private ObservableCollection<ChatMessage> _allchatMessages;
        public ObservableCollection<ChatMessage> AllChatMessages
        {
            get => _allchatMessages;
            set => SetProperty(ref _allchatMessages, value);
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

        private int _onlineCount;
        public int OnlineCount
        {
            get => _onlineCount;
            set => SetProperty(ref _onlineCount, value);
        }

        public RelayCommand SendNewPostCommand { get; set; }
        public RelayCommand OpenChatCommand { get; set; }
        public AsyncRelayCommand<PostBrief> OpenPostDetailCommandAsync { get; set; }
        //public AsyncRelayCommand LoadedCommandAsync { get; set; }
        public AsyncRelayCommand<PostBrief> GoodCommandAsync { get; set; }
        public RelayCommand<string> ViewImageCommand { get; set; }
        public AsyncRelayCommand SendMessageCommandAsync { get; set; }
        public RelayCommand GroupMessageCommand { get; set; }
        public RelayCommand LoadHistoryMessageCommmand { get; set; }

        public TeamupViewModel(Post post,
                               EnumHelper enumHelper,
                               IniSettingsModel iniSettingsModel,
                               IConfiguration configuration,
                               ITeamupService teamupService)
        {
            Page = 1;
            _post = post;
            _teamupService = teamupService;
            _configuration = configuration;
            _iniSettingsModel = iniSettingsModel;
            PostCategories = enumHelper.GetEnumItemValueDesc(typeof(PostCategory));
            OpenPostDetailCommandAsync = new AsyncRelayCommand<PostBrief>(OpenPostDetail);
            SendNewPostCommand = new RelayCommand(SendNewPost);
            //LoadedCommandAsync = new AsyncRelayCommand(LoadedAsync);
            GoodCommandAsync = new AsyncRelayCommand<PostBrief>(GoodAsync);
            ViewImageCommand = new RelayCommand<string>(ViewImage);
            OpenChatCommand = new RelayCommand(OpenChat);
            SendMessageCommandAsync = new AsyncRelayCommand(SendMessageAsync);
            GroupMessageCommand = new RelayCommand(GroupMessage);
            LoadHistoryMessageCommmand = new RelayCommand(LoadHistoryMessage);
            ChatMessages = new ObservableCollection<ChatMessage>();
            ChatMessages.Add(new ChatMessage() { IsLoadData = true });
            AllChatMessages = new ObservableCollection<ChatMessage>();
        }

        private void SendNewPost()
        {
            _post.ShowDialog();
        }

        public void SetOnlineCount(int value) 
        {
            OnlineCount = value;
            App.ServiceProvider.GetRequiredService<MainWindowViewModel>().OnlineCounts = OnlineCount;
        }

        private void GroupMessage()
        {
            if (string.IsNullOrEmpty(_iniSettingsModel.ChatMessageTemplate))
            {
                ChatMessage = $"来自{Constant.Account.ServerArea}的{Constant.Account.DisplayName}请求一起打游戏";
            }
            else
            {
                var template = _iniSettingsModel.ChatMessageTemplate;
                ChatMessage = template.Replace("{name}", Constant.Account.DisplayName)
                        .Replace("{solorank}", $"{Constant.Account.Rank.RANKED_SOLO_5x5.CnTier} {Constant.Account.Rank.RANKED_SOLO_5x5.Division}")
                        .Replace("{flexrank}", $"{Constant.Account.Rank.RANKED_FLEX_SR.CnTier} {Constant.Account.Rank.RANKED_FLEX_SR.Division}")
                        .Replace("{serverarea}", Constant.Account.ServerArea);
            }
        }

        private void LoadHistoryMessage()
        {
            var msg = ChatMessages.Count() > 1 ? ChatMessages[1] : null;
            if (msg == null)
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "没有更多历史聊天记录",
                    ShowDateTime = false
                });

                return;
            }

            var index = AllChatMessages.IndexOf(msg);
            if (index >= 20)
            {
                foreach (var tempIndex in Enumerable.Range(0, 20))
                {
                    ChatMessages.Insert(1, AllChatMessages[index - tempIndex - 1]);
                }
            }
            else if (index > 0 && index < 20)
            {
                for (var temp = index - 1; temp >= 0; temp--)
                {
                    ChatMessages.Insert(1, AllChatMessages[temp]);
                }
            }
            else
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "没有更多历史聊天记录",
                    ShowDateTime = false
                });

                return;
            }
        }

        public void AddNewMessage(ChatMessage msg)
        {
            msg.CurrentIsAdministrator = Constant.Account.IsAdministrator;
            msg.IsAdministrator = msg.Role.Contains("Administrator");
            msg.IsSender = msg.UserId == Constant.Account.SummonerId;
            ChatMessages.Add(msg);
            AllChatMessages.Add(msg);
            double bottomOffset = ChatScrollViewer.ExtentHeight - ChatScrollViewer.VerticalOffset - ChatScrollViewer.ViewportHeight;
            if (ChatScrollViewer.VerticalOffset > 0 && bottomOffset == 0 && ChatMessages.Count > 50)
            {
                ChatMessages.RemoveAt(1);
            }
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

        //TODO后期酌情上线
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
            if (_lastSendTime != null && DateTime.Now.AddSeconds(-3) <= _lastSendTime.Value)
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "发送消息过于频繁,过几秒再试!",
                    ShowDateTime = false
                });

                return;
            }
            if (string.IsNullOrEmpty(ChatMessage) || ChatMessage.Length > 50)
            {
                return;
            }

            await App.HubConnection.SendAsync("SendMessage", ChatMessage);
            ChatMessage = null;
            _lastSendTime = DateTime.Now;
        }
    }
}
