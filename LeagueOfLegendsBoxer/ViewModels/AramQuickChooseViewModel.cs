using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class AramQuickChooseViewModel : ObservableObject
    {
        public AsyncRelayCommand SelectHerosLockCommandAsync { get; set; }
        public AsyncRelayCommand UnSelectHerosLockCommandAsync { get; set; }
        public RelayCommand LoadCommand { get; set; }

        private ObservableCollection<Hero> _quickChooseHeros;
        public ObservableCollection<Hero> QuickChooseHeros
        {
            get => _quickChooseHeros;
            set => SetProperty(ref _quickChooseHeros, value);
        }

        private ObservableCollection<Hero> _subQuickChooseHeros;
        public ObservableCollection<Hero> SubQuickChooseHeros
        {
            get => _subQuickChooseHeros;
            set => SetProperty(ref _subQuickChooseHeros, value);
        }

        private ObservableCollection<Hero> _selectedQuickChooseHeros;
        public ObservableCollection<Hero> SelectedQuickChooseHeros
        {
            get => _selectedQuickChooseHeros;
            set => SetProperty(ref _selectedQuickChooseHeros, value);
        }

        private ObservableCollection<Hero> _subSelectedQuickChooseHeros;
        public ObservableCollection<Hero> SubSelectedQuickChooseHeros
        {
            get => _subSelectedQuickChooseHeros;
            set => SetProperty(ref _subSelectedQuickChooseHeros, value);
        }

        private readonly IniSettingsModel _iniSettingsModel;
        public AramQuickChooseViewModel(IniSettingsModel iniSettingsModel)
        {
            _iniSettingsModel = iniSettingsModel;
            LoadCommand = new RelayCommand(Load);
            SelectHerosLockCommandAsync = new AsyncRelayCommand(SelectHerosLockAsync);
            UnSelectHerosLockCommandAsync = new AsyncRelayCommand(UnSelectHerosLockAsync);
        }

        private void Load() 
        {
            QuickChooseHeros = new ObservableCollection<Hero>(Constant.Heroes.Where(x => !_iniSettingsModel.LockHerosInAram.Contains(x.ChampId)).OrderBy(x => x.Name));
            SubQuickChooseHeros = new ObservableCollection<Hero>();
            SubSelectedQuickChooseHeros = new ObservableCollection<Hero>();
            var list = new List<Hero>();
            foreach (var item in _iniSettingsModel.LockHerosInAram)
            {
                var h = Constant.Heroes.FirstOrDefault(x => x.ChampId == item);
                if (h != null)
                {
                    list.Add(h);
                }
            }
            SelectedQuickChooseHeros = new ObservableCollection<Hero>(list);
        }

        private async Task SelectHerosLockAsync()
        {
            if (SubQuickChooseHeros.Count <= 0)
                return;

            if (SubQuickChooseHeros.Count + SelectedQuickChooseHeros.Count > 20)
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "乱斗模式秒选最多设置20位英雄",
                    ShowDateTime = false
                });

                return;
            }
            var temp = new List<Hero>();
            foreach (var item in SubQuickChooseHeros)
            {
                temp.Add(item);
            }

            foreach (var item in temp)
            {
                QuickChooseHeros.Remove(item);
                SelectedQuickChooseHeros.Add(item);
            }

            await _iniSettingsModel.WriteLockHerosInAramAsync(SelectedQuickChooseHeros.Select(x => x.ChampId).ToList());
            SubQuickChooseHeros.Clear();
        }

        public async Task WriteIntoSetting() 
        {
            await _iniSettingsModel.WriteLockHerosInAramAsync(SelectedQuickChooseHeros.Select(x => x.ChampId).ToList());
        }

        private async Task UnSelectHerosLockAsync()
        {
            if (SubSelectedQuickChooseHeros.Count <= 0)
                return;

            var temp = new List<Hero>();
            foreach (var item in SubSelectedQuickChooseHeros)
            {
                temp.Add(item);
            }

            foreach (var item in temp)
            {
                QuickChooseHeros.Add(item);
                SelectedQuickChooseHeros.Remove(item);
            }
            QuickChooseHeros = new ObservableCollection<Hero>(QuickChooseHeros.OrderBy(x => x.Name));
            await _iniSettingsModel.WriteLockHerosInAramAsync(SelectedQuickChooseHeros.Select(x => x.ChampId).ToList());
            SubSelectedQuickChooseHeros.Clear();
        }
    }
}
