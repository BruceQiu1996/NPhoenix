using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class MainViewModel :  ObservableObject
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
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        private readonly IAccountService _accountService;
        private readonly IGameService _gameService;
        private readonly ILogger<MainViewModel> _logger;
       
        public MainViewModel(IAccountService accountService, ILogger<MainViewModel> logger, IGameService gameService)
        {
            _accountService = accountService;
            _gameService = gameService;
            _logger = logger;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {

            try
            {
                var profile = await _accountService.GetUserAccountInformationAsync();
                Account = JsonSerializer.Deserialize<Account>(profile);
                var rankData = JToken.Parse(await _accountService.GetUserRankInformationAsync());
                Account.Rank = rankData["queueMap"].ToObject<Rank>();
                var recordsData = JToken.Parse(await _accountService.GetRecordInformationAsync(Account.SummonerId));
                Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
