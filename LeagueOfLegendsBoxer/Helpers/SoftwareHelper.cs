using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class SoftwareHelper
    {
        private readonly ILogger<SoftwareHelper> _logger;
        private readonly ITeamupService _teamupService;

        public SoftwareHelper(ILogger<SoftwareHelper> logger, ITeamupService teamupService)
        {
            _logger = logger;
            _teamupService = teamupService;
        }

        //获取软件更新
        public async Task Update()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe");
            var updateFile = files.FirstOrDefault(f => f.Split('\\').LastOrDefault() == "NPhoenixAutoUpdateTool.exe");
            if (updateFile != null)
            {
                Process.Start(updateFile, version);
            }
            else
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var responseMessage = await client.GetAsync("http://www.dotlemon.top:5200/upload/NPhoenix/NPhoenixAutoUpdateTool.exe", HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
                            responseMessage.EnsureSuccessStatusCode();
                            if (responseMessage.StatusCode == HttpStatusCode.OK)
                            {
                                var filePath = Directory.GetCurrentDirectory() + "/NPhoenixAutoUpdateTool.exe";
                                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                                {
                                    await responseMessage.Content.CopyToAsync(fs);
                                }
                                if (File.Exists(filePath))
                                {
                                    Process.Start(filePath, version);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("下载自动更新程序失败");
                        _logger.LogError(ex, "下载自动更新程序失败:");
                    }
                });
            }
        }

        //设置开机自启
        public void RunAtStart(bool isAuto)
        {
            var starupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LeagueOfLegendsBoxer.exe");
            try
            {
                var fileName = starupPath;
                var shortFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                //打开子键节点
                var myReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                if (myReg == null)
                {
                    //如果子键节点不存在，则创建之
                    myReg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (myReg != null && myReg.GetValue(shortFileName) != null)
                {
                    //在注册表中设置自启动程序
                    myReg.DeleteValue(shortFileName);
                    myReg.SetValue(shortFileName, fileName);
                    _logger.LogInformation("设置自启动程序操作成功");
                }
                else if (myReg != null && myReg.GetValue(shortFileName) == null)
                {
                    myReg.SetValue(shortFileName, fileName);
                    _logger.LogInformation("设置自启动程序操作成功");
                }
            }
            catch
            {
                _logger.LogInformation("写注册表操作发生错误");
            }
        }
        public async Task<TotalRank> GetRanksAsync()
        {
            try
            {
                var result = await _teamupService.GetRankDataAsync();
                if (string.IsNullOrEmpty(result))
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "拉取排位信息失败",
                        ShowDateTime = false
                    });

                    return null;
                }

                var totalRank = JsonConvert.DeserializeObject<TotalRank>(result);
                var mvpRank = 0;
                foreach (var item in totalRank.Mvp)
                {
                    item.Rank = ++mvpRank;
                }
                var svpRank = 0;
                foreach (var item in totalRank.Svp)
                {
                    item.Rank = ++svpRank;
                }
                var noobRank = 0;
                foreach (var item in totalRank.Noob)
                {
                    item.Rank = ++noobRank;
                }
                var xiaguRank = 0;
                foreach (var item in totalRank.Xiagu)
                {
                    item.Rank = ++xiaguRank;
                }
                var aramRank = 0;
                foreach (var item in totalRank.Aram)
                {
                    item.Rank = ++aramRank;
                }

                return totalRank;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取排位信息失败",
                    ShowDateTime = false
                });

                return null;
            }
        }

        private readonly string _regPath = "SOFTWARE\\NPHOENIX\\WindowBounds\\";
        public void SaveSize(System.Windows.Window window) => Registry.CurrentUser.CreateSubKey(_regPath + window.Name).SetValue("Bounds", window.RestoreBounds.ToString());

        public void SetSize(System.Windows.Window window)
        {
            try
            {
                if (window.SizeToContent != SizeToContent.Manual)
                {
                    return;
                }

                var bounds = Registry.CurrentUser.OpenSubKey(_regPath + window.Name);
                if (bounds is null)
                    return;

                var value = bounds.GetValue("Bounds").ToString();
                var rect = value.Split(",");

                window.Top = double.Parse(rect[1]);
                window.Left = double.Parse(rect[0]);
                window.Width = double.Parse(rect[2]);
                window.Height = double.Parse(rect[3]);
            }
            catch  { }
        }
    }
}
