using System;
using System.IO.IsolatedStorage;
using System.Net;

namespace OpenStreetApp
{
    public static class FileDownloader
    {
        public static void download(Uri uri, string path, Action<string> callback)
        {
            var wc = new WebClient();
            var token = new path_lock();
            token.path = path;
            token.callback = callback;
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            wc.OpenReadAsync(uri, token);
        }

        static void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var token = (path_lock)e.UserState;

            byte[] buffer = new byte[512];
            int len;
            var isf = IsolatedStorageFile.GetUserStoreForApplication();
            var fout = isf.CreateFile(token.path);
            while ((len = e.Result.Read(buffer, 0, buffer.Length)) > 0)
            {
                fout.Write(buffer, 0, len);
            }
            fout.Flush();
            fout.Close();
            e.Result.Close();

            token.callback(token.path);
        }

        private struct path_lock
        {
            public string path;
            public Action<string> callback;
        }
    }
}