using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class SummonerAnalyseViewModel : ObservableObject
    {
        private List<Page> _pages = new List<Page>();

        private Page _currentPage;
        public Page CurrentPage 
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private string _searchName;
        public string SearchName
        {
            get => _searchName;
            set => SetProperty(ref _searchName, value);
        }

        public AsyncRelayCommand SearchRecordByNameAsyncCommand { get; set; }

        private readonly IAccountService _accountService;
        private readonly ILogger<SummonerAnalyseViewModel> _logger;

        public SummonerAnalyseViewModel(IAccountService accountService, ILogger<SummonerAnalyseViewModel> logger)
        {
            SearchRecordByNameAsyncCommand = new AsyncRelayCommand(SearchRecordByNameAsync);
            _accountService = accountService;
            _logger = logger;
        }

        public void LoadPageByAccount(Account account) 
        {
            var summonerDetail = App.ServiceProvider.GetRequiredService<SummonerDetail>();
            var summonerDetailViewModel = App.ServiceProvider.GetRequiredService<SummonerDetailViewModel>();
            summonerDetailViewModel.Account = account;
            summonerDetailViewModel.SummonerAnalyseViewModel = this;
            summonerDetail.DataContext = summonerDetailViewModel;
            CurrentPage = summonerDetail;
            _pages.Add(summonerDetail);
        }

        public async Task LoadPageAsync(long summonerId) 
        {
            var infromation = await _accountService.GetSummonerInformationAsync(summonerId);
            var account = JsonConvert.DeserializeObject<Account>(infromation);
            var rankData = JToken.Parse(await _accountService.GetSummonerRankInformationAsync(account.Puuid));
            account.Rank = rankData["queueMap"].ToObject<Rank>();
            var recordsData = JToken.Parse(await _accountService.GetRecordInformationAsync(account.SummonerId));
            account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());

            var summonerDetail = App.ServiceProvider.GetRequiredService<SummonerDetail>();
            var summonerDetailViewModel = App.ServiceProvider.GetRequiredService<SummonerDetailViewModel>();
            summonerDetailViewModel.Account = account;
            summonerDetailViewModel.SummonerAnalyseViewModel = this;
            summonerDetail.DataContext = summonerDetailViewModel;
            CurrentPage = summonerDetail;
            _pages.Add(summonerDetail);
        }

        private async Task SearchRecordByNameAsync() 
        {
            if (string.IsNullOrEmpty(SearchName.Trim()))
                return;

            try
            {
                var data = await _accountService.GetSummonerInformationAsync(SearchName.Trim());
                if (data == null)
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "查无此人",
                        ShowDateTime = false
                    });
                }
                else
                {
                    var id = JToken.Parse(data)["accountId"].ToObject<long>();
                    await LoadPageAsync(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "查询账号信息失败!" + ex.Message,
                    ShowDateTime = false
                });
            }
        }
    }
}
