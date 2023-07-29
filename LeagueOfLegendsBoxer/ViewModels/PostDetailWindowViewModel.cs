using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Models.V2;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class PostDetailWindowViewModel : ObservableObject
    {
        private readonly ITeamupService _teamupService;

        private Post _post;
        public Post Post 
        {
            get => _post;
            set => SetProperty(ref _post, value);
        }
        private readonly ILogger<PostDetailWindowViewModel> _logger;

        public AsyncRelayCommand GoodCommandAsync { get; set; }

        public PostDetailWindowViewModel(ITeamupService teamupService,
                                         ILogger<PostDetailWindowViewModel> logger)
        {
            _logger = logger;
            _teamupService = teamupService;
            GoodCommandAsync = new AsyncRelayCommand(GoodAsync);
        }

        internal async Task LoadPostDetailAsync(Post post)
        {
            Post = post;

            await Task.CompletedTask;
        }

        private async Task GoodAsync() 
        {
            try
            {
                var result = await _teamupService.GoodAsync(Post.Id);
                Post.HadGood = result.Item1;
                Post.GoodCount = result.Item2;
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
    }
}
