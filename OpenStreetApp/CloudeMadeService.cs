using System;
using System.Net;

namespace OpenStreetApp
{
    public static class CloudeMadeService
    {
        public static string Token { get; private set; }
        public static string UserId { get; private set; }
        public const string ApiKey = "1a8bcc813f9646519c9d2b12e92c69b2";

        static CloudeMadeService()
        {
            var r = new Random();
            UserId = r.Next(100000, 999999).ToString();
        }

        public static void authorize(Action callback)
        {
            // skip request if we already are authorized
            if (string.IsNullOrEmpty(Token) == false)
            {
                callback();
                return;
            }

            var uri = new Uri("http://auth.cloudmade.com/token/" + ApiKey + "?userid=" + UserId);
            var req = HttpWebRequest.CreateHttp(uri);
            req.Method = "POST";
            req.BeginGetResponse((a) =>
            {
                if (a.IsCompleted)
                {
                    var stream = req.EndGetResponse(a).GetResponseStream();
                    var buffer = new byte[512];
                    var len = stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    var result = System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, len);
                    Token = result;
                    callback();
                }
            }, null);
        }
    }
}
