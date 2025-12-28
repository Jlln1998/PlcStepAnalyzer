using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Events;
using Serilog;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace PlcStepAnalyzer.Pages.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEventAggregator eventAggregator)
        {
            // 根据系统DPI调整窗口大小
            InitializeComponent();
            Log.Information("MainWindow - 初始化");
            Loaded += (obj, e) =>
            {
                // 主屏幕工作区尺寸（逻辑像素，排除任务栏）
                double primaryWorkWidth = SystemParameters.WorkArea.Width;
                double primaryWorkHeight = SystemParameters.WorkArea.Height;

                this.Width = primaryWorkWidth * 4 / 5;
                this.Height = primaryWorkHeight * 4 / 5;

                eventAggregator.GetEvent<NavToMenuEvent>().Publish((GlobalData.Instance.SysMenus.FirstOrDefault(), null));
            };
            this.PreviewMouseDown += (sender, e) =>
            {
                if (!(sender is TextBox || sender is PasswordBox ||
                    sender is RichTextBox || sender is ComboBox ||
                    sender is ListBox || sender is ListView ||
                    sender is DataGrid || sender is DatePicker ||
                    sender is CheckBox || sender is RadioButton))
                {
                    Keyboard.ClearFocus();
                }
            };
        }

        public static string REGION_MAIN { get; } = "MainRegion";

        /// <summary>
        /// 最小化、最大化、关闭等按钮触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowControlBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name.ToString())
                {
                    case "MiniSizeBtn":
                        this.WindowState = WindowState.Minimized;
                        break;
                    case "MaxOrResizeBtn":
                        this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                        break;
                    case "CloseBtn":

                        App.Current.Shutdown();
                        break;
                    default:
                        break;
                }
            }
        }

        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "https://github.com/Jlln1998/PlcStepAnalyzer";

                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开链接失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}