using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Models
{
  public class Response<T>
  {
    public ResponseCode Code { get; set; }

    public string Message { get; set; }

    public bool Success { get; set; }

    public T Data { get; set; }
  }

  public enum ResponseCode
  {
    [Description("通用错误")]
    Fail = -1,

    [Description("成功")]
    Success = 200,

    [Description("登录失败")]
    LoginFail = 201,

    [Description("未授权")]
    Unauthorized = 401,

    [Description("未知异常")]
    UnkonwnEx = 500
  }
}
