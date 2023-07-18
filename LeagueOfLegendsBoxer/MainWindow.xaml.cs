using CommunityToolkit.Mvvm.Messaging;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeagueOfLegendsBoxer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IniSettingsModel _iniSettingsModel;

        public MainWindow(MainWindowViewModel mainWindowViewModel, IniSettingsModel iniSettingsModel)
        {
            InitializeComponent();
            DataContext = mainWindowViewModel;
            _iniSettingsModel = iniSettingsModel;

            WeakReferenceMessenger.Default.Register<MainWindow, string,string>(this, "back",async (x, y) =>
            {
                if (string.IsNullOrEmpty(y))
                {
                    this.render.Visibility = Visibility.Visible;
                    this.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                    await _iniSettingsModel.WriteBackgroundImage(string.Empty);
                }
                else 
                {
                    this.render.Visibility = Visibility.Hidden;
                    ImageBrush b = new ImageBrush();
                    b.ImageSource = new BitmapImage(new Uri(y,UriKind.Absolute));
                    b.Stretch = Stretch.Fill;
                    Background = b;
                    await _iniSettingsModel.WriteBackgroundImage(y);
                }
            });
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Label_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();
        }

        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Topmost = true;
            var _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Topmost = false;
                });
            });
        }

        private void Label_MouseLeftButtonDown_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
    }
}
