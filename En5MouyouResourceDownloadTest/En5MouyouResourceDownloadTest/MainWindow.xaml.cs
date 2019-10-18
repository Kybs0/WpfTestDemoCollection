using System;
using System.Threading.Tasks;
using System.Windows;

namespace En5MouyouResourceDownloadTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DownloadUrlTextBox.Text =
                "http://static.cvte.com/file/myou/uploads/in_app_update/7dba5d0bcfcb408be66e1e4b6a52eeb54a34c900/59a392c0d57ed33333dd281c/59a39-0.0.0.10.zip";
            DownloadLogTextBox.Text +=
                "古诗词资源：http://static.cvte.com/file/myou/uploads/in_app_update/7dba5d0bcfcb408be66e1e4b6a52eeb54a34c900/59a392c0d57ed33333dd281c/59a39-0.0.0.10.zip\r\n";
            DownloadLogTextBox.Text +=
                "英汉字典资源：http://static.cvte.com/file/myou/uploads/android_rom/7dba5d0bcfcb408be66e1e4b6a52eeb54a34c900/English_Dict.0.0.7-ba204.zip\r\n";

        }

        private async void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = DownloadUrlTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var downloadUrl = text;

                try
                {
                    DownloadLogTextBox.Text += $"正在下载{downloadUrl}\r\n";
                    var filePath=await Task.Run(() =>
                    {
                        WebResourceDownloadHelper.Download(downloadUrl, out var downloadedPath);
                        return downloadedPath;
                    });
                    DownloadLogTextBox.Text += $"下载完成，文件保存路径：{filePath}\r\n";
                }
                catch (Exception exception)
                {
                    DownloadLogTextBox.Text += $"下载失败：{exception.Message}\r\n";
                }
            }
        }
    }
}
