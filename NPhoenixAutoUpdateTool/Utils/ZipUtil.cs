using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Utils
{
  public class ZipUtil
  {
    /// <summary>
    /// 解压缩,拷贝,删除
    /// </summary>
    /// <param name="sourcePath">zip的路径</param>
    /// <param name="targetPath">目的路径</param>
    /// <returns></returns>
    public static bool UnZip(string sourcePath, string targetPath)
    {
      try
      {
        string zipFile = Path.Combine(sourcePath, "temp.zip");
        string extractPath = Path.Combine(targetPath, "temp");
        if (!Directory.Exists(extractPath))
        {
          Directory.CreateDirectory(extractPath);
        }
        ZipFile.ExtractToDirectory(zipFile, extractPath);//将zip文件拷贝到临时文件夹
        if (Directory.Exists(Path.Combine(extractPath, "SeriesApp")))
        {
          extractPath = Path.Combine(extractPath, "SeriesApp");
        }
        //将临时文件夹下的文件复制到原程序路径中
        CopyDirectory(extractPath, sourcePath);//注意,此时临时文件夹为源地址,sourcePath为目标地址
        File.Delete(zipFile);//删除zip文件
        Directory.Delete(Path.Combine(targetPath, "temp"), true);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// 递归copy文件
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="targetPath"></param>
    private static void CopyDirectory(string sourcePath, string targetPath)
    {
      try
      {
        if (!Directory.Exists(targetPath))
        {
          Directory.CreateDirectory(targetPath);
        }
        string[] files = Directory.GetFiles(sourcePath);//Copy文件
        foreach (string file in files)
        {
          try
          {
            string pFilePath = targetPath + "\\" + Path.GetFileName(file);
            File.Copy(file, pFilePath, true);
          }
          catch (Exception)
          {
            continue;
          }
        }

        string[] dirs = Directory.GetDirectories(sourcePath);//Copy目录
        foreach (string dir in dirs)
        {
          CopyDirectory(dir, targetPath + "\\" + Path.GetFileName(dir));
        }
      }
      catch (Exception ex)
      {

      }
    }
  }
}
}
