using Gma.System.MouseKeyHook;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// SelectKey.xaml 的交互逻辑
    /// </summary>
    public partial class SelectKey : Window
    {
        public string SelectKeys { get; set; }
        private IKeyboardMouseEvents _keyboardMouseEvents;

        private List<string> select = new List<string>(); 

        public SelectKey()
        {
            InitializeComponent();
            this._keyboardMouseEvents = Hook.GlobalEvents();
            _keyboardMouseEvents.KeyDown += _keyboardMouseEvents_KeyDown;

        }

        private void _keyboardMouseEvents_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var key = e.KeyCode.ToString();
            if (key.Contains("Control"))
            {
                key = "Control";
            }
            else if (key.Contains("Menu"))
            {
                key = "Alt";
            }

            if (!select.Contains(key))
            {
                select.Add(key);

                SelectKeys = string.Join('+', select);

                tb.Text = SelectKeys;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(SelectKeys))
            {
                HandyControl.Controls.MessageBox.Show("请按下快捷键");
                return;
            }
            _keyboardMouseEvents.Dispose();
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            _keyboardMouseEvents.Dispose();
            this.DialogResult = false;
            this.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            select.Clear();
            SelectKeys = "";
            tb.Text = "";
        }

        public IEnumerable<string> GetKeys()
        {
            return SelectKeys.Split('+');
        }
    }
}
