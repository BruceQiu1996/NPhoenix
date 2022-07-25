using LeagueOfLegendsBoxer.ViewModels.Pages;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
        }

        private void ComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key==Key.Down)
                e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/BruceQiu1996/NPhoenix";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //不使用shell启动
                p.StartInfo.RedirectStandardInput = true;//喊cmd接受标准输入
                p.StartInfo.RedirectStandardOutput = false;//不想听cmd讲话所以不要他输出
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示窗口
                p.Start();//向cmd窗口发送输入信息 后面的&exit告诉cmd运行好之后就退出
                p.StandardInput.WriteLine("start " + url + "&exit");
                p.StandardInput.AutoFlush = true;
                p.WaitForExit();
                p.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/BruceQiu1996/NPhoenix/tree/master/Runes";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //不使用shell启动
                p.StartInfo.RedirectStandardInput = true;//喊cmd接受标准输入
                p.StartInfo.RedirectStandardOutput = false;//不想听cmd讲话所以不要他输出
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示窗口
                p.Start();//向cmd窗口发送输入信息 后面的&exit告诉cmd运行好之后就退出
                p.StandardInput.WriteLine("start " + url + "&exit");
                p.StandardInput.AutoFlush = true;
                p.WaitForExit();
                p.Close();
            }
        }
    }
}
