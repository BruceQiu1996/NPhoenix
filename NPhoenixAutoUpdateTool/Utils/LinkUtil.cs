using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Utils
{
  public class LinkUtil
  {
    [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
    private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
    private const int MAX_PATH = 260;
    private const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019;
    public static string GetAllUsersDesktopFolderPath()
    {
      StringBuilder sbPath = new StringBuilder(MAX_PATH);
      SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_DESKTOPDIRECTORY, IntPtr.Zero, 0, sbPath);
      return sbPath.ToString();
    }

    /// <summary>
    /// 获取桌面所有快捷方式
    /// </summary>
    /// <returns></returns>
    public static string[] GetDesktopLink()
    {
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      return Directory.GetFiles(desktopPath, "*.lnk");
    }

    /// <summary>
    /// 创建桌面快捷方式
    /// </summary>
    /// <param name="shortcutName">快捷方式名称</param>
    /// <param name="targetPath">目标路径</param>
    /// <param name="description">描述</param>
    /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"</param>
    /// <remarks></remarks>
    public static void CreateLinkToDesktop(string linkName, string targetPath, string description = null, string iconLocation = null)
    {
      string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//获取桌面文件夹路径

      CreateShortcut(desktop, linkName, targetPath, description, iconLocation);
    }

    /// <summary>
    /// 创建快捷方式
    /// </summary>
    /// <param name="directory">快捷方式所处的文件夹</param>
    /// <param name="shortcutName">快捷方式名称</param>
    /// <param name="targetPath">目标路径</param>
    /// <param name="description">描述</param>
    /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，
    /// 例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
    /// <remarks></remarks>
    public static void CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
    {
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
      WshShell shell = new WshShell();
      IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
      shortcut.TargetPath = targetPath;//指定目标路径
      shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
      //shortcut.WindowStyle = ;//设置运行方式，默认为常规窗口
      shortcut.Description = description;//设置备注
      shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
      shortcut.Save();//保存快捷方式
    }
  }
}
