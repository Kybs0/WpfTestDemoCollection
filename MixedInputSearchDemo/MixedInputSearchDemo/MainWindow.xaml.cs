using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MixedInputSearchDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RegexTextBox.Text = ChineseChars;
        }
        /// <summary>
        /// 英文字母与数字
        /// </summary>
        public const string LettersAndNumbers = "[a-zA-Z0-9]+";

        /// <summary>
        /// 中文字符
        /// </summary>
        public const string ChineseChars = "[\u4E00-\u9FA5]+";

        /// <summary>
        /// 英文字符
        /// </summary>
        public const string EnglishChars = "[a-zA-Z]+";

        private void CheckChineseButton_OnClick(object sender, RoutedEventArgs e)
        {
            RegexTextBox.Text = ChineseChars;
            var match = Regex.Match(InpuTextBox.Text, ChineseChars, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                ResultTextBlock.Text = $"Index:{match.Index},Length:{match.Length}\r\nResult:{match.Value}";
            }
            else
            {
                ResultTextBlock.Text = string.Empty;
            }
        }

        private void CheckEnglishButton_OnClick(object sender, RoutedEventArgs e)
        {
            RegexTextBox.Text = EnglishChars;
            var match = Regex.Match(InpuTextBox.Text, EnglishChars, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                ResultTextBlock.Text = $"Index:{match.Index},Length:{match.Length}\r\nResult:{match.Value}";
            }
            else
            {
                ResultTextBlock.Text = string.Empty;
            }
        }

        private void CheckNumbersButton_OnClick(object sender, RoutedEventArgs e)
        {
            RegexTextBox.Text = LettersAndNumbers;
            var match = Regex.Match(InpuTextBox.Text, LettersAndNumbers, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                ResultTextBlock.Text = $"Index:{match.Index},Length:{match.Length}\r\nResult:{match.Value}";
            }
            else
            {
                ResultTextBlock.Text = string.Empty;
            }
        }

        private void CheckAllCharsButton_OnClick(object sender, RoutedEventArgs e)
        {
            RegexTextBox.Text = EnglishChars;
            var handledText = InpuTextBox.Text;
            var match = Regex.Matches( handledText, EnglishChars, RegexOptions.IgnoreCase);
            if (match.Count>0)
            {
                var matches = match.Cast<Match>().ToList();
                ResultTextBlock.Text = string.Empty;
                foreach (var matchIn in matches)
                {
                    ResultTextBlock.Text += $"Index:{matchIn.Index},Length:{matchIn.Length}\r\nResult:{matchIn.Value}\r\n";
                }
            }
            else
            {
                ResultTextBlock.Text = string.Empty;
            }
        }
    }
}
