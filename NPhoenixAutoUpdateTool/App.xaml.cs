using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
    private void Application_Startup(object sender, StartupEventArgs e)
    {
      if(e.Args.Length == 0)
      {
        this.Shutdown();
      }

      if(Regex.IsMatch(e.Args[0], "^(((ht|f)tp[s]?):\\/\\/)?([^!@#$%^&*?.\\s-]([^!@#$%^&*?.\\s]{0,63}[^!@#$%^&*?.\\s])?\\.)+[a-z]{2,6}\\/?"))
      {
        Global.UpdateUrl = e.Args[0];
        MessageBox.Show(Global.UpdateUrl);
      }

      this.Shutdown();
    }
  }
}
