using System;
using System.Collections.Generic;
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
using CefSharp;
using CefSharp.Wpf.Experimental;

namespace WpfCefBrowserDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TheBrowserControl.Load(UrlTextBox.Text);
        }
    }

    public class ChromiumWebBrowserWithTouchSupport11 : ChromiumWebBrowserWithTouchSupport
    {
        public ChromiumWebBrowserWithTouchSupport11()
        {
            IsBrowserInitializedChanged += CefBrowserOnIsBrowserInitializedChanged;
        }
        private void CefBrowserOnIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ChromiumWebBrowserWithTouchSupport chromiumWebBrowser)
            {
                //chromiumWebBrowser.ShowDevTools();
            }
        }
    }
}
