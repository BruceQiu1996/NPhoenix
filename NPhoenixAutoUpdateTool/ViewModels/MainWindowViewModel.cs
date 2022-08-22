using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NPhoenixAutoUpdateTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NPhoenixAutoUpdateTool.Utils;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace NPhoenixAutoUpdateTool.ViewModels
{
  public class MainWindowViewModel : ObservableObject
  {
    private double _progressValue;
    public double ProgressValue
    {
      get => _progressValue;
      set => SetProperty(ref _progressValue, value);
    }

    private string _currentSize;
    public string CurrentSize
    {
      get => _currentSize;
      set => SetProperty(ref _currentSize, value);
    }

    private string _totalSize;
    public string TotalSize
    {
      get => _totalSize;
      set
      {
        if(value != _totalSize)
        {
          SetProperty(ref _totalSize, value);
        }
      }
    }

    private string _percentage;
    public string Percentage
    {
      get => _percentage;
      set
      {
        SetProperty(ref _percentage, value);
      }
    }

    public RelayCommand LoadedCommand { get; set; }


    public MainWindowViewModel()
    {
      LoadedCommand = new RelayCommand(Loaded);
    }

    private async void Loaded()
    {
      if(MessageBox.Show("发现新版本是否更新?","更新",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        ProcessUtil.KillProcessByName("LeagueOfLegendsBoxer");
        await DownLoadFileAsync();
      }

      // 退出程序
      Application.Current.Shutdown();

    }

    private async Task DownLoadFileAsync()
    {
      HttpClient httpClient = new HttpClient();
      Progress progress = new Progress();
      progress.Handler += Progress_Handler;
      var bytes = await httpClient.GetByteArrayAsync(new Uri("http://www.dotlemon.top:5200/upload/NPhoenix/NPhoenix2.3.zip"), progress, CancellationToken.None);
      var pathDir = Directory.GetCurrentDirectory() + "\\temp";
      if (!Directory.Exists(pathDir))
      {
        Directory.CreateDirectory(pathDir);
      }

      var path = pathDir + "\\temp.zip";
      if (File.Exists(path))
      {
        File.Delete(path);
      }

      Percentage = "正在写入...";
      using (FileStream fileStream = new FileStream(path, FileMode.Create))
      {
        await fileStream.WriteAsync(bytes, 0, bytes.Length);
      }

      // 开始解压
      await UnzipFileAsync(path, Directory.GetCurrentDirectory());

      var lolBoxerPath = Directory.GetCurrentDirectory() + "/LeagueOfLegendsBoxer.exe";

      Percentage = "创建图标...";
      // 获取桌面所有图标
      var links = LinkUtil.GetDesktopLink();
      var link = links.FirstOrDefault(l => l == "LeagueOfLegendsBoxer");
      if (link != null)
      {
        File.Delete(link);
      }

      // 创建图标
      LinkUtil.CreateLinkToDesktop("LeagueOfLegendsBoxer", lolBoxerPath, "LOL", lolBoxerPath);


      Percentage = "运行程序...";
      // 运行助手
      Process.Start(lolBoxerPath);
    }

    private async Task UnzipFileAsync(string filePath,string fileDir)
    {
      Percentage = "开始解压...";
      await Task.Run(() =>
      {
        ZipUtil.UnZip(filePath, fileDir);
      });
    }

    private void Progress_Handler(HttpDownloadProgress progress)
    {
      CurrentSize = (progress.BytesReceived / 1024 / 1024.0).ToString("0.00") + "MB";
      if (progress.TotalBytesToReceive != null)
      {
        TotalSize = (progress.TotalBytesToReceive.Value / 1024 / 1024.0).ToString("0.00") + "MB";
        var value = 100.0 * progress.BytesReceived / progress.TotalBytesToReceive;
        ProgressValue = value.Value;
        Percentage = value.Value.ToString("0.00") + " %";
      }
    }
  }
}
