using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class NoticeViewModel : ObservableObject
    {
        private bool _isLoaded = false;

        private ObservableCollection<Notice> notices;
        public ObservableCollection<Notice> Notices
        {
            get { return notices; }
            set { SetProperty(ref notices, value); }
        }

        private Notice notice;
        public Notice Notice
        {
            get { return notice; }
            set { SetProperty(ref notice, value); }
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILogger<NoticeViewModel> _logger;
        public NoticeViewModel(IConfiguration configuration, ILogger<NoticeViewModel> logger)
        {
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            _configuration = configuration;
            _logger = logger;
        }

        private async Task LoadAsync()
        {
            if (_isLoaded)
                return;

            var notice = _configuration.GetSection("NoticeLocation").Value;
            if (string.IsNullOrEmpty(notice))
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "未能拉取通告信息",
                    ShowDateTime = false
                });

                return;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    var data = await client.GetStringAsync(notice);
                    if (string.IsNullOrEmpty(data)) 
                    {
                        return;
                    }
                    var notices = JsonConvert.DeserializeObject<IEnumerable<Notice>>(data);
                    Notices = new ObservableCollection<Notice>(notices.OrderBy(x => x.Priority).ThenByDescending(x => x.Time));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "获取通告发生错误",
                        ShowDateTime = false
                    });
                }
            }
        }
    }
}
