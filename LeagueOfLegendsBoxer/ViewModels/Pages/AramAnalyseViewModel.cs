using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class AramAnalyseViewModel : ObservableObject
    {
        private ObservableCollection<AramChampDescModel> _chooseChamps;
        public ObservableCollection<AramChampDescModel> ChooseChamps
        {
            get => _chooseChamps;
            set => SetProperty(ref _chooseChamps, value);
        }

        private ObservableCollection<AramChampDescModel> _benchChamps;
        public ObservableCollection<AramChampDescModel> BenchChamps
        {
            get => _benchChamps;
            set => SetProperty(ref _benchChamps, value);
        }

        public AramAnalyseViewModel()
        {
            ChooseChamps = new ObservableCollection<AramChampDescModel>();
            BenchChamps = new ObservableCollection<AramChampDescModel>();
            WeakReferenceMessenger.Default.Register<AramAnalyseViewModel, AramChooseHeroModel>(this, (x, y) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ChooseChamps.Clear();
                    BenchChamps.Clear();
                    foreach (var item in y.ChampIds)
                    {
                        ChooseChamps.Add(new AramChampDescModel()
                        {
                            Id = item,
                            Buff = Constant.AramBuffs.FirstOrDefault(x => x.Id == item.ToString()),
                        });
                    }

                    foreach (var item in y.BenchChamps)
                    {
                        BenchChamps.Add(new AramChampDescModel()
                        {
                            Id = item,
                            Buff = Constant.AramBuffs.FirstOrDefault(x => x.Id == item.ToString()),
                        });
                    }
                });
            });
        }
    }
}
