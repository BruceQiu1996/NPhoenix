using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            set
            {
                if (value != null)
                {
                    //设置该id的通知为已读
                    SetReadedAsync(value).ConfigureAwait(false);
                }
                SetProperty(ref notice, value);
            }
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ILogger<NoticeViewModel> _logger;
        public NoticeViewModel(IConfiguration configuration, ILogger<NoticeViewModel> logger, IniSettingsModel iniSettingsModel)
        {
            Notices = new ObservableCollection<Notice>();
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            _configuration = configuration;
            _logger = logger;
            _iniSettingsModel = iniSettingsModel;
        }

        public async Task LoadAsync()
        {
            Notice = null;
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
                    var data = await client.GetByteArrayAsync(notice);
                    if (data == null || data.Count() <= 0)
                    {
                        return;
                    }
                    var dataStr = Encoding.UTF8.GetString(data);
                    var notices = JsonConvert.DeserializeObject<IEnumerable<Notice>>(dataStr);
                    foreach(var item in notices) 
                    {
                        if (_iniSettingsModel.ReadedNotices.Contains(item.Id)) 
                        {
                            item.IsReaded = true;
                        }
                    }
                    Notices = new ObservableCollection<Notice>(notices.OrderBy(x => x.Priority).ThenByDescending(x => x.Time));
                    WeakReferenceMessenger.Default.Send(Notices.Where(x => !x.IsReaded));
                    _isLoaded = true;
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
        private async Task SetReadedAsync(Notice notice)
        {
            if (!_iniSettingsModel.ReadedNotices.Contains(notice.Id))
            {
                await _iniSettingsModel.WriteReadedNoticesAsync(notice.Id);
            }

            notice.IsReaded = true;
            WeakReferenceMessenger.Default.Send(Notices.Where(x => !x.IsReaded));
        }
    }
}
