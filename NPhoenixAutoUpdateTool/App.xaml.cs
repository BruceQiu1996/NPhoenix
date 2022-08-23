using Newtonsoft.Json;
using NPhoenixAutoUpdateTool.Models;
using NPhoenixAutoUpdateTool.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NPhoenixAutoUpdateTool
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
      // 获取软件外传递来的版本号
      string version = string.Empty;
      if (e.Args.Length > 0)
      {
        version = e.Args[0];
        LogUtil.WriteInfo(version);
      }

      try
      {
        // 获取最新数据
        var nphoenix = await HttpClientUtil.FindLastDataAsync();
        if (nphoenix != null)
        {
          LogUtil.WriteInfo(JsonConvert.SerializeObject(nphoenix));
          if (string.IsNullOrWhiteSpace(version) || nphoenix.Version != version)
          {
            Global.NPhoenix = nphoenix;
            return;
          }
        }
      }
      catch (Exception ex)
      {
        LogUtil.WriteError(ex, "获取最新数据异常!");
        this.Shutdown();
      }

      // 获取不到数据 关闭软件
      this.Shutdown();
    }
  }
}
