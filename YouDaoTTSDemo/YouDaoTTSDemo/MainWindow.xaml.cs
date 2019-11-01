using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YouDaoTTSDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyTextBox.Text = "You have to take capital appreciation of the property into account.\r\n" +
                             "The direction of the prevailing winds should be taken into account.\r\n" +
                             "The Villad'Este was a short distance northeast of Rome, nestled high in the Sabine Hills.\r\n";
        }

        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();
            string url = "https://openapi.youdao.com/ttsapi";
            string q = MyTextBox.Text;
            string appKey = "75766d8fc97f34a3";
            string appSecret = "rFkTqsDws1bCoETcxSL7afG33emwJdr5";
            string salt = DateTime.Now.Millisecond.ToString();
            string langType = "en-USA";
            dic.Add("langType", langType);
            string signStr = appKey + q + salt + appSecret; ;
            string sign = ComputeHash(signStr, new MD5CryptoServiceProvider());
            dic.Add("q", System.Web.HttpUtility.UrlEncode(q));
            dic.Add("appKey", appKey);
            dic.Add("salt", salt);
            dic.Add("sign", sign);
            Post(url, dic);
        }

        protected static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        protected static void Post(string url, Dictionary<String, String> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if (resp.ContentType.ToLower().Equals("audio/mp3"))
            {
                SaveBinaryFile(resp);
            }
            else
            {
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                Console.WriteLine(result);
            }
        }

        protected static string Truncate(string q)
        {
            if (q == null)
            {
                return null;
            }
            int len = q.Length;
            return len <= 20 ? q : (q.Substring(0, 10) + len + q.Substring(len - 10, 10));
        }

        private static bool SaveBinaryFile(WebResponse response)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{Guid.NewGuid()}.mp3";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                Stream outStream = System.IO.File.Create(filePath);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();

                MessageBox.Show($"保存录音文件成功，保存路径：{filePath}");
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }

}
