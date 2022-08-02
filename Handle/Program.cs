using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public class Program
{
    // 查找窗口
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // 遍历窗口的所有子窗口，通过CallBack回调
    [DllImport("user32.dll")]
    public static extern int EnumChildWindows(IntPtr hWndParent, CallBack lpfn, int lParam);
    public delegate bool CallBack(IntPtr hwnd, int lParam);

    // 获取窗口的类名
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    // 判断窗口是否可见
    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    // 获取窗口文本长度
    [DllImport("user32.dll")]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    // 获取窗口文本，文本会塞入StringBuilder中，需要指明字符串最大长度nMaxCount
    [DllImport("User32.dll", EntryPoint = "GetWindowText")]
    private static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("User32.dll ")]
    public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

    const int WM_GETTEXT = 0x000D;
    const int WM_GETTEXTLENGTH = 0x000E;
    const int WM_CLOSE = 0x10;
    const int WM_KEYDOWN = 0x0100;

    /// <summary>
    /// 尝试查找Error窗口并取出窗口文本
    /// </summary>
    /// <returns></returns>
    public static string TryFindErrorWindowText()
    {
        SendkEys
    }

    public static void Main(string[] args) 
    {
        TryFindErrorWindowText();
    }

    public class Win32API
    {
        [DllImport("user32.dll ", CharSet = CharSet.Unicode)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    }
}