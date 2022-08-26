using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixDownloader.Utils
{
  public class LogUtil
  {
    /// <summary>
    /// 输入异常到日志
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="message"></param>
    public static void WriteError(Exception ex, string message)
    {
      var dirPath = CheckDir();
      string msg = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\r\n{message}\r\n{ex.Message}\r\n{ex.StackTrace}\r\n***********************************\r\n";
      File.AppendAllText(dirPath + "\\Error.log", msg);
    }

    /// <summary>
    /// 输出信息到日志
    /// </summary>
    /// <param name="message"></param>
    public static void WriteInfo(string message)
    {
      var dirPath = CheckDir();
      string msg = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\r\n{message}\r\n****************************************\r\n";
      File.AppendAllText(dirPath + "\\Info.log", msg);
    }

    private static string CheckDir()
    {
      var dirPath = Directory.GetCurrentDirectory() + "\\AutoUpdateLog";
      if(!Directory.Exists(dirPath))
      {
        Directory.CreateDirectory(dirPath);
      }

      return dirPath;
    }
  }
}
