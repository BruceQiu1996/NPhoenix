using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NPhoenixDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NPhoenixDownloader.Utils;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace NPhoenixDownloader.ViewModels
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
        if (value != _totalSize)
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

    private string _title = "下载";
    public string Title
    {
      get => _title;
      set => SetProperty(ref _title, value);
    }

    public RelayCommand LoadedCommand { get; set; }
    private string filePath = string.Empty;


    public MainWindowViewModel()
    {
      LoadedCommand = new RelayCommand(Loaded);
    }

    private async void Loaded()
    {
      try
      {
        if (Global.NPhoenix != null)
        {
          Title = $"下载 版本: V{Global.NPhoenix.Version}";
          CommonOpenFileDialog dialog = new CommonOpenFileDialog();
          dialog.IsFolderPicker = true;
          if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
          {
            filePath = dialog.FileName;
            await UpdateDownLoadNumberAsync();
            await RunUpdateAsync();
          }
        }
      }
      catch (Exception ex)
      {
        LogUtil.WriteError(ex, "下载操作文件异常");
      }

      // 退出程序
      Application.Current.Shutdown();

    }

    private async Task UpdateDownLoadNumberAsync()
    {
      using (HttpClient httpClient = new HttpClient())
      {
        var httpResponseMessage = await httpClient.GetAsync($"http://www.dotlemon.top:5200/NPhoenix/DownLoadFileById?id={Global.NPhoenix.Id}");
        if (httpResponseMessage != null && httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
          var json = await httpResponseMessage.Content.ReadAsStringAsync();
          var response = JsonConvert.DeserializeObject<Response<object>>(json);
          if (response.Code != ResponseCode.Success)
          {
            LogUtil.WriteInfo(json);
          }
        }
      }
    }

    /// <summary>
    /// 开始运行更新
    /// </summary>
    /// <returns></returns>
    private async Task RunUpdateAsync()
    {
      // 下载文件字节数据
      var bytes = await DownLoadFileAsync();

      // 检查文件目录
      var path = CheckDir();

      Percentage = "正在写入...";
      using (FileStream fileStream = new FileStream(path, FileMode.Create))
      {
        await fileStream.WriteAsync(bytes, 0, bytes.Length);
      }

      Percentage = "开始解压...";
      await UnzipFileAsync(path, filePath);

      var lolBoxerPath = $"{filePath}/{Global.NPhoenix.StartName}";

      Percentage = "创建图标...";
      CreateLink(lolBoxerPath);

      Percentage = "运行程序...";
      Process.Start(lolBoxerPath);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <returns></returns>
    private async Task<byte[]> DownLoadFileAsync()
    {
      HttpClient httpClient = new HttpClient();
      Progress progress = new Progress();
      progress.Handler += Progress_Handler;
      return await httpClient.GetByteArrayAsync(new Uri(Global.NPhoenix.DownUrl), progress, CancellationToken.None);
    }

    /// <summary>
    /// 检查文件目录和多余文件
    /// </summary>
    /// <returns>文件路径</returns>
    private string CheckDir()
    {
      var pathDir = filePath + "\\temp";
      if (!Directory.Exists(pathDir))
      {
        Directory.CreateDirectory(pathDir);
      }

      var path = pathDir + "\\temp.zip";
      if (File.Exists(path))
      {
        File.Delete(path);
      }

      return path;
    }

    /// <summary>
    /// 解压文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileDir"></param>
    /// <returns></returns>
    private async Task UnzipFileAsync(string filePath, string fileDir)
    {
      await Task.Run(() =>
      {
        ZipUtil.UnZip(filePath, fileDir);
      });
    }

    /// <summary>
    /// 创建图标
    /// </summary>
    /// <param name="lolBoxerPath"></param>
    private void CreateLink(string lolBoxerPath)
    {
      var linkName = Global.NPhoenix.LinkName == null ? "LeagueOfLegendsBoxer" : Global.NPhoenix.LinkName;
      // 获取桌面所有图标
      var links = LinkUtil.GetDesktopLink();
      var link = links.FirstOrDefault(l => l == linkName);
      if (link != null)
      {
        File.Delete(link);
      }

      // 创建图标
      LinkUtil.CreateLinkToDesktop(linkName, lolBoxerPath, "LOL", lolBoxerPath);
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
