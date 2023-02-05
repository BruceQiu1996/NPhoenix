using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LeagueOfLegendsBoxer.Models
{
    public class ChatMessage
    {
        public bool IsLoadData { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public string ServerArea { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Message { get; set; }
        public bool IsSender { get; set; }
        public bool IsAdministrator { get; set; }
        public bool CurrentIsAdministrator { get; set; }
        public string Role { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeText => ConvertDateTimeToText(CreateTime);
        public RelayCommand CopyCurrentUserNameCommand { get; set; }
        public AsyncRelayCommand DenySendMessageCommandAsync { get; set; }
        public AsyncRelayCommand OpenRecordByIdCommandAsync { get; set; }

        public ChatMessage()
        {
            CopyCurrentUserNameCommand = new RelayCommand(() =>
            {
                Clipboard.SetText(UserName);
            });

            DenySendMessageCommandAsync = new AsyncRelayCommand(async () =>
            {
                var data = await App.ServiceProvider.GetRequiredService<ITeamupService>().DenyChatAsync(UserId);
                if (data)
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "禁言成功",
                        ShowDateTime = false
                    });
                }
                else
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "服务器错误",
                        ShowDateTime = false
                    });
                }
            });

            OpenRecordByIdCommandAsync = new AsyncRelayCommand(async () =>
            {
                if (string.IsNullOrEmpty(ServerArea) || string.IsNullOrEmpty(Constant.Account.ServerArea) || ServerArea != Constant.Account.ServerArea) 
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "不在同一个服务器无法查询战绩,或者你和查询对象有一人暂未设置所在大区,请到设置里进行服务器设置。",
                        ShowDateTime = false
                    });

                    return;
                }

                var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
                var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
                await summonerAnalyseViewModel.LoadPageAsync(UserId);
                summonerAnalyse.Show();
            });
        }

        //TODO 统一方法
        public string ConvertDateTimeToText(DateTime dateTime)
        {
            if (dateTime.Year == DateTime.Now.Year && dateTime.DayOfYear == DateTime.Now.DayOfYear)
            {
                //今天
                return dateTime.ToString("t");
            }
            else if (dateTime.Year == DateTime.Now.Year)
            {
                //今年
                return dateTime.ToString("MM-dd HH:mm");
            }
            else
            {
                return dateTime.ToString("yyyy/MM/dd HH:mm");
            }
        }
    }
}
