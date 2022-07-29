using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPhoenixSPA.Models;
using NPhoenixSPA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NPhoenixSPA.ViewModels
{
    public class AccountViewModel : ObservableObject
    {
        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private ObservableCollection<Record> _records;
        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        private ObservableCollection<Friend> _friends;
        public ObservableCollection<Friend> Friends
        {
            get => _friends;
            set => SetProperty(ref _friends, value);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand<long> SubscribeFriendCommandAsync { get; set; }
        public AsyncRelayCommand<long> UnSubscribeFriendCommandAsync { get; set; }

        private readonly IAccountService _accountService;
        private readonly ILogger<AccountViewModel> _logger;
        private readonly IniSettingsModel _iniSettingsModel;

        public AccountViewModel(IAccountService accountService,
                                IniSettingsModel iniSettingsModel,
                                ILogger<AccountViewModel> logger)
        {
            _accountService = accountService;
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            SubscribeFriendCommandAsync = new AsyncRelayCommand<long>(SubscribeFriendAsync);
            UnSubscribeFriendCommandAsync = new AsyncRelayCommand<long>(UnSubscribeFriendAsync);
        }

        public async Task LoadAsync()
        {
            try
            {
                var profile = await _accountService.GetUserAccountInformationAsync();
                Account = JsonConvert.DeserializeObject<Account>(profile);
                Constant.Account = Account;
                await _iniSettingsModel.RefreshSubscribeFriendsList();
                var recordsData = JToken.Parse(await _accountService.GetRecordInformationAsync(Account.SummonerId));
                Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
                var rankData = JToken.Parse(await _accountService.GetUserRankInformationAsync());
                Account.Rank = rankData["queueMap"].ToObject<Rank>();
                var friends = await _accountService.GetFriendsAsync();
                var friendList = JsonConvert.DeserializeObject<IEnumerable<Friend>>(friends).OrderByDescending(x => x.Time);
                foreach (var friend in friendList) 
                {
                    if (_iniSettingsModel.SubscribeFriendsList.Contains(friend.SummonerId))
                        friend.IsSubscribe = true;
                }

                Friends = new ObservableCollection<Friend>(friendList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取登录信息失败,可以点击刷新重试",
                    ShowDateTime = false
                });
            }
        }

        public async Task SubscribeFriendAsync(long id)
        {
            try
            {
                var ids = Friends.Where(x => x.IsSubscribe).Select(x => x.SummonerId).ToList();
                if (!ids.Contains(id)) { ids.Add(id); }
                await _iniSettingsModel.WriteSubscribeFriendsListAsync(ids);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = $"订阅好友{Friends.FirstOrDefault(x => x.SummonerId == id)?.Name}成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "订阅好友信息失败",
                    ShowDateTime = false
                });
            }
        }

        public async Task UnSubscribeFriendAsync(long id)
        {
            try
            {
                var ids = Friends.Where(x => x.IsSubscribe).Select(x => x.SummonerId).ToList();
                ids.Remove(id);
                await _iniSettingsModel.WriteSubscribeFriendsListAsync(ids);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = $"取消订阅好友{Friends.FirstOrDefault(x => x.SummonerId == id)?.Name}成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "取消订阅好友信息失败",
                    ShowDateTime = false
                });
            }
        }
    }
}
