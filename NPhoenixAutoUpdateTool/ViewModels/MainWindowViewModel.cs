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
        if(value != _percentage)
        {
          SetProperty(ref _percentage, value);
        }
      }
    }

    public RelayCommand LoadedCommand { get; set; }


    public MainWindowViewModel()
    {
      LoadedCommand = new RelayCommand(Loaded);
    }

    private async void Loaded()
    {
      ProcessUtil.KillProcessByName("LeagueOfLegendsBoxer");
      await DownLoadFileAsync();
    }

    private async Task DownLoadFileAsync()
    {
      HttpClient httpClient = new HttpClient();
      Progress progress = new Progress();
      progress.Handler += Progress_Handler;
      var bytes = await httpClient.GetByteArrayAsync(new Uri("http://www.dotlemon.top:5200/upload/NPhoenix/NPhoenix2.2.zip"), progress, CancellationToken.None);
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
      using (FileStream fileStream = new FileStream(path, FileMode.Create))
      {
        Percentage = "正在写入...";
        await fileStream.WriteAsync(bytes, 0, bytes.Length);
        Percentage = "写入完成...";
      }

      // 开始解压
      UnzipFile(path,Directory.GetCurrentDirectory());
    }

    private void UnzipFile(string filePath,string fileDir)
    {
      Percentage = "开始解压...";
      ZipUtil.UnZip(filePath, fileDir);
      Percentage = "解压完成...";
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
