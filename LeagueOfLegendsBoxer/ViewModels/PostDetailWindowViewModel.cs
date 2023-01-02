using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class PostDetailWindowViewModel : ObservableObject
    {
        private int _pageSize = 10;

        private readonly ITeamupService _teamupService;
        private readonly IConfiguration _configuration;
        private readonly EnumHelper _enumHelper;
        private readonly ILogger<PostDetailWindowViewModel> _logger;

        public Dictionary<string, string> PostCategories { get; set; }

        private PostDetail _postDetail;
        public PostDetail PostDetail
        {
            get => _postDetail;
            set => SetProperty(ref _postDetail, value);
        }

        private string _commmentContent;
        public string CommmentContent
        {
            get => _commmentContent;
            set => SetProperty(ref _commmentContent, value);
        }

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

        public AsyncRelayCommand CommentCommandAsync { get; set; }
        public AsyncRelayCommand<FunctionEventArgs<int>> JumpCommentsPageCommandAsync { get; set; }
        public AsyncRelayCommand GoodCommandAsync { get; set; }
        public RelayCommand<string> ViewImageCommand { get; set; }

        public PostDetailWindowViewModel(ITeamupService teamupService,
                                         EnumHelper enumHelper,
                                         ILogger<PostDetailWindowViewModel> logger,
                                         IConfiguration configuration)
        {
            _logger = logger;
            _teamupService = teamupService;
            _configuration = configuration;
            _enumHelper = enumHelper;
            PostCategories = _enumHelper.GetEnumItemValueDesc(typeof(PostCategory));
            CommentCommandAsync = new AsyncRelayCommand(CommentAsync);
            JumpCommentsPageCommandAsync = new AsyncRelayCommand<FunctionEventArgs<int>>(JumpCommentsPageAsync);
            GoodCommandAsync = new AsyncRelayCommand(GoodAsync);
            ViewImageCommand = new RelayCommand<string>(ViewImage);
        }

        internal async Task LoadPostDetailAsync(long postId)
        {
            var prefix = _configuration.GetSection("PostImagesPrefix").Value;
            var post = await _teamupService.GetPostDetailAsync(postId);
            PostDetail = new PostDetail()
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
                ServerArea = post.ServerArea,
                Rank_FLEX_SR = post.Rank_FLEX_SR,
                Rank_SOLO_5x5 = post.Rank_SOLO_5x5,
                PostCategoryText = PostCategories[((int)post.PostCategory).ToString()],
                Image_1_loc = string.IsNullOrEmpty(post.Image_1) ? null : $"{prefix}{post.Image_1}",
                Image_2_loc = string.IsNullOrEmpty(post.Image_2) ? null : $"{prefix}{post.Image_2}",
                Image_3_loc = string.IsNullOrEmpty(post.Image_3) ? null : $"{prefix}{post.Image_3}",
            };

            int index = 0;
            PostDetail.PostComments = new ObservableCollection<PostComment>(post.PostCommentDtos.Select(x => new PostComment()
            {
                Publisher = x.Publisher,
                CreateTime = x.CreateTime,
                CommentContent = x.CommentContent,
                UserName = x.UserName,
                ServerArea = x.ServerArea,
                Rank_FLEX_SR = x.Rank_FLEX_SR,
                Rank_SOLO_5x5 = x.Rank_SOLO_5x5,
                Desc = x.Desc,
                Index = ++index
            }));

            AllPages = PostDetail.CommmentsCount / _pageSize;
            if (PostDetail.CommmentsCount % _pageSize != 0)
                AllPages++;
            Page = 1;
        }

        private async Task JumpCommentsPageAsync(FunctionEventArgs<int> e)
        {
            await GetCommentsByPage(e.Info);
        }

        private async Task GetCommentsByPage(int page) 
        {
            var data = await _teamupService.GetPostCommentsByPage(PostDetail.Id, page);
            int index = 0;

            PostDetail.PostComments = new ObservableCollection<PostComment>(data.Data.Select(x => new PostComment()
            {
                Publisher = x.Publisher,
                CreateTime = x.CreateTime,
                CommentContent = x.CommentContent,
                UserName = x.UserName,
                ServerArea = x.ServerArea,
                Rank_FLEX_SR = x.Rank_FLEX_SR,
                Rank_SOLO_5x5 = x.Rank_SOLO_5x5,
                Desc = x.Desc,
                Index = ++index
            }));
        }

        private async Task GoodAsync() 
        {
            try
            {
                var result = await _teamupService.GoodAsync(PostDetail.Id);
                PostDetail.HadGood = result.Item1;
                PostDetail.GoodCount = result.Item2;
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

        private async Task CommentAsync()
        {
            try
            {
                var result = await _teamupService.CreatePostCommentAsync(new CreatePostCommentDto()
                {
                    PostId = PostDetail.Id,
                    Content = CommmentContent
                });

                if (result)
                {
                    Growl.SuccessGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "评论成功",
                        ShowDateTime = false
                    });

                    CommmentContent = null;
                    if (Page == 1) //刷新第一页评论区
                    {
                        await GetCommentsByPage(1);
                    }
                }
                else
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "发生错误",
                        ShowDateTime = false
                    });
                }
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "发生错误",
                    ShowDateTime = false
                });

                _logger.LogError(ex.ToString());
            }
        }

        private void ViewImage(string imgLoc) 
        {
            if(string.IsNullOrEmpty(imgLoc))
                return;

            new ImageBrowser(new Uri(imgLoc)).Show();
        }
    }
}
