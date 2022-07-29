using CommunityToolkit.Mvvm.ComponentModel;
using NPhoenixSPA.Models;
using System.Collections.ObjectModel;

namespace NPhoenixSPA.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Menu> _menus;
        public ObservableCollection<Menu> Menus 
        {
            get { return _menus; }
            set { SetProperty(ref _menus, value); }
        }

        public MainWindowViewModel()
        {
            Menus = new ObservableCollection<Menu>()
            {
                new Menu()
                {
                    Name = "账号信息",
                    Icon = System.Windows.Application.Current.FindResource("gitIcon")
                    
                },
                new Menu()
                {
                    Name = "国服数据",
                    Icon = System.Windows.Application.Current.FindResource("gitIcon")

                },
                new Menu()
                {
                    Name = "符文数据",
                    Icon = System.Windows.Application.Current.FindResource("gitIcon")
                },
                new Menu()
                {
                    Name = "设置",
                    Icon = System.Windows.Application.Current.FindResource("gitIcon")
                },
            };
        }
    }
}
