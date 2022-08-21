using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Utils
{
  public static class HttpClientUtil
  {
    // 每次读取 128kb
    private const int BufferSize = 1024 * 128;

    public static async Task<byte[]> GetByteArrayAsync(this HttpClient client, Uri requestUri, IProgress<HttpDownloadProgress> progress, CancellationToken cancellationToken)
    {
      if (client == null)
      {
        throw new ArgumentNullException(nameof(client));
      }

      // 构建发送的消息
      using (var responseMessage = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
      {
        responseMessage.EnsureSuccessStatusCode();

        var content = responseMessage.Content;
        if (content == null)
        {
          return Array.Empty<byte>();
        }

        // 获取参数
        var headers = content.Headers;
        var contentLength = headers.ContentLength;
        // 读取数据
        using (var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false))
        {
          var buffer = new byte[BufferSize];
          int bytesRead;
          var bytes = new List<byte>();

          // 读取一次
          var downloadProgress = new HttpDownloadProgress();
          if (contentLength.HasValue)
          {
            downloadProgress.TotalBytesToReceive = (ulong)contentLength.Value;
          }
          progress?.Report(downloadProgress);

          // 循环读取
          while ((bytesRead = await responseStream.ReadAsync(buffer, 0, BufferSize, cancellationToken).ConfigureAwait(false)) > 0)
          {
            bytes.AddRange(buffer.Take(bytesRead));

            downloadProgress.BytesReceived += (ulong)bytesRead;
            progress?.Report(downloadProgress);
          }

          return bytes.ToArray();
        }
      }
    }
  }

  public struct HttpDownloadProgress
  {
    public ulong BytesReceived { get; set; }

    public ulong? TotalBytesToReceive { get; set; }
  }
}
