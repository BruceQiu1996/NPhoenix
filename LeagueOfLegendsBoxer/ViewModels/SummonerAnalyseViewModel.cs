using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private Account _account;
        public Account Account 
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public RelayCommand LoadCommand { get; set; }

        private readonly IAccountService _accountService;
        public SummonerAnalyseViewModel(IAccountService accountService)
        {
            LoadCommand = new RelayCommand(Load);
            _accountService = accountService;
        }

        public void Load() 
        {
            var summonerDetail = App.ServiceProvider.GetRequiredService<SummonerDetail>();
            var summonerDetailViewModel = App.ServiceProvider.GetRequiredService<SummonerDetailViewModel>();
            summonerDetailViewModel.Account = Account;
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
    }
}
