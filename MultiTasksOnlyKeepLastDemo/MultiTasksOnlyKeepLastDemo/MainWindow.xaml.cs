using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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

namespace MultiTasksOnlyKeepLastDemo
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

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var asyncTaskQueue = new AsyncTaskQueue
            {
                AutoCancelPreviousTask = true,
                UseSingleThread = true
            })
            {
                // 快速启动20个任务
                for (var i = 1; i < 20; i++)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    Test(asyncTaskQueue, i);
                }
            }
        }

        public static async void Test(AsyncTaskQueue taskQueue, int num)
        {
            var result = await taskQueue.ExecuteAsync(async () =>
            {
                // 长时间耗时任务
                await Task.Delay(TimeSpan.FromSeconds(5));
                Debug.WriteLine("输入:" + num);
                return num * 100;
            });
            Debug.WriteLine($"{num}输出的:" + result);
        }

    }
}
