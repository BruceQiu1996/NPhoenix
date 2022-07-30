using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Game;
using Newtonsoft.Json.Linq;
using NPhoenixSPA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NPhoenixSPA.ViewModels
{
    public class SkinsWindowViewModel : ObservableObject
    {
        private readonly IGameService _gameService;

        private ObservableCollection<Skin> _skins;
        public ObservableCollection<Skin> Skins
        {
            get => _skins;
            set => SetProperty(ref _skins, value);
        }

        private Skin _skin;
        public Skin Skin
        {
            get => _skin;
            set
            {
                SetProperty(ref _skin, value);
            }
        }
        public AsyncRelayCommand SetBackgroundImageCommandAsync { get; set; }
        public SkinsWindowViewModel(IGameService gameService)
        {
            _gameService = gameService;
            SetBackgroundImageCommandAsync = new AsyncRelayCommand(SetBackgroundImageAsync);
        }

        public async Task<(bool,string)> LoadSkinsAsync(Hero hero) 
        {
            try
            {
                var result = await _gameService.GetSkinsByHeroId(hero.ChampId);
                if (!string.IsNullOrEmpty(result))
                {
                    var skins = JToken.Parse(result)["skins"].ToObject<IEnumerable<Skin>>();
                    Skins = new ObservableCollection<Skin>(skins);

                    foreach (var skin in skins)
                    {
                        var bytes = await _gameService.GetResourceByUrl(skin.SplashPath);
                        skin.Image = ByteArrayToBitmapImage(bytes);
                    }

                    return (true, null);
                }

                return (false, "无法找到皮肤信息");
            }
            catch (Exception ex) 
            {
                return (false, ex.Message);
            }
        }
        private async Task SetBackgroundImageAsync() 
        {
            if (Skin == null)
                return;

            var result = await _gameService.SetSkinAsync(new
            {
                key = "backgroundSkinId",
                value = Skin.Id
            });
        }

        public BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}
