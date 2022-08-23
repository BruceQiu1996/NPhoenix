using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Utils
{
  public static class ProcessUtil
  {
    /// <summary>
    /// 根据进程名杀死某个进程
    /// </summary>
    /// <param name="processName"></param>
    public static void KillProcessByName(string processName)
    {
      if(processName.Contains('.'))
      {
        processName = processName.Split('.')[0];
      }
      var processs = Process.GetProcessesByName(processName);
      foreach (var process in processs)
      {
        process.Kill();
      }
    }
  }
}
