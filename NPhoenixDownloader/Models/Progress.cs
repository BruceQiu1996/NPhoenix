using NPhoenixDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixDownloader.Models
{
  /// <summary>
  /// 通知进度
  /// </summary>
  public class Progress : IProgress<HttpDownloadProgress>
  {
    public event Action<HttpDownloadProgress> Handler;

    public void Report(HttpDownloadProgress value)
    {
      Handler?.Invoke(value);
    }
  }
}
