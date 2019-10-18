using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace En5MouyouResourceDownloadTest
{
    public class WebResourceDownloadHelper
    {
        public static bool Download(string resourceUri, out string downloadPath)
        {
            downloadPath = string.Empty;
            if (string.IsNullOrWhiteSpace(resourceUri))
            {
                return false;
            }
            try
            {
                //"http://ydschool-online.nos.netease.com/account_v0205.mp3"
                WebRequest request = WebRequest.Create(resourceUri);
                WebResponse response = request.GetResponse();
                using (Stream reader = response.GetResponseStream())
                {
                    var userDownloadFolder = GetUserDownloadFolder();
                    downloadPath = Path.Combine(userDownloadFolder, $"{DateTime.Now.Ticks}-{Path.GetFileName(resourceUri)}");
                    if (File.Exists(downloadPath))
                    {
                        File.Delete(downloadPath);
                    }
                    using (FileStream writer = new FileStream(downloadPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buff = new byte[512];
                        int c = 0;                                           //实际读取的字节数   
                        while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, c);
                        }
                        response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            //下载成功
            return true;
        }

        private static string GetUserDownloadFolder()
        {
            string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            return strDesktopPath;
        }
    }
}
