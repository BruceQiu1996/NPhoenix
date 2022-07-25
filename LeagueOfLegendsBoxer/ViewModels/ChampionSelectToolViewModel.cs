using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Pages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Menu = LeagueOfLegendsBoxer.Models.Menu;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class ChampionSelectToolViewModel : ObservableObject
    {
        private Menu _currentMenu;
        public Menu CurrentMenu
        {
            get => _currentMenu;
            set
            {
                if (_currentMenu != value)
                    value.Action?.Invoke();
                SetProperty(ref _currentMenu, value);
            }
        }

        private ObservableCollection<Menu> _menus;
        public ObservableCollection<Menu> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private readonly RunePage _runePage;
        public ChampionSelectToolViewModel(HeroData heroData, Teammate teammate, RunePage runePage)
        {
            _runePage = runePage;
            Menus = new ObservableCollection<Menu>()
            {
                new Menu()
                {
                    Name = "英雄数据",
                    Action = ()=>CurrentPage = heroData
                },
                new Menu()
                {
                    Name = "队友信息",
                    Action = ()=>CurrentPage = teammate
                },
                new Menu()
                {
                    Name = "符文配置",
                    Action = ()=>CurrentPage = runePage
                }
            };
            CurrentMenu = Menus.FirstOrDefault();
        }

        public void ShowRunePage()
        {
            CurrentMenu = Menus.LastOrDefault();
        }
    }
}
