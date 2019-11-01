using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBox1.Text = "You have to take capital appreciation of the property into account.\r\n" +
                                        "The direction of the prevailing winds should be taken into account.\r\n" +
                                        "The Villad'Este was a short distance northeast of Rome, nestled high in the Sabine Hills.\r\n";

            Loaded += MainWindow_Loaded;
        }

        private void TestSpeechButton_OnClick(object sender, RoutedEventArgs e)
        {
            PlayAsync();
        }

        /// <summary>
        /// 异步播放
        /// </summary>
        private void PlayAsync()
        {
            try
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 导出音频文件
        /// </summary>
        private void ExportAudioButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{Guid.NewGuid()}.mp3";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var speed = SpeedSlide.Value;
                var rate = RateSlide.Value;
                var voice = (int)VoiceComboBox.SelectedValue;

                var option = new Dictionary<string, object>()
                {
                    {"spd", speed}, // 语速
                    //{"vol", voice}, // 音量
                    {"pit", rate}, // 音量
                    {"per", voice}  // 发音人，4：情感度丫丫童声
                };
                var result = _ttsClient.Synthesis(TextBox1.Text, option);

                if (result.ErrorCode == 0)  // 或 result.Success
                {
                    File.WriteAllBytes(filePath, result.Data);
                }

                MessageBox.Show($"保存录音文件成功，保存路径：{filePath}");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void VoiceComboBox_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlayAsync();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置APPID/AK/SK
            _ttsClient = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
            _ttsClient.Timeout = 60000;  // 修改超时时间
        }
        // 0为女声，1为男声，3为情感合成-度逍遥，4为情感合成-度丫丫，默认为普通女
        public List<SpeechVoice> SpeechVoices => new List<SpeechVoice>(){
            new SpeechVoice()
            {
                    Id = 0,
                    Name = "女声"
            },
            new SpeechVoice()
            {
                Id = 1,
                Name = "男声"
            },
            new SpeechVoice()
            {
                Id = 3,
                Name = "情感-度逍遥"
            },            new SpeechVoice()
            {
                Id = 4,
                Name = "情感-度丫丫"
            },
        };

        private void RateSlide_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayAsync();
        }

        private void SpeedSlide_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayAsync();
        }

        private Baidu.Aip.Speech.Tts _ttsClient;
        const string APP_ID = "14640485";
        const string API_KEY = "VsWMOM5spqcAF5qlufuwgQlq";
        const string SECRET_KEY = "4mp6ntGsWnu4dqWLsWmHsWjKr4oen5ww";
    }

    public class SpeechVoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
