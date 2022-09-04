using Newtonsoft.Json;
using NPhoenixDownloader.Models;
using NPhoenixDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NPhoenixDownloader
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      try
      {
        // 获取最新数据
        var nphoenix = await HttpClientUtil.FindLastDataAsync();
        if (nphoenix == null)
        {
          // 获取不到数据 关闭软件
          this.Shutdown();
        }
        else
        {
          //LogUtil.WriteInfo(JsonConvert.SerializeObject(nphoenix));
          Global.NPhoenix = nphoenix;
          var mainWindow = new MainWindow();
          mainWindow.Show();
        }
      }
      catch (Exception ex)
      {
        LogUtil.WriteError(ex, "获取最新数据异常!");
        this.Shutdown();
      }
    }
  }
}
