using Serilog;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace PlcStepAnalyzer
{
    public partial class App
    {

        /// <summary>
        /// UI 线程（Dispatcher）未处理异常
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                ParseExceptionMsg(e.Exception, "UI 线程异常");
            }
            catch (Exception ex)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 所有线程未处理的全局异常（AppDomain 级别）
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                ParseExceptionMsg(exception, "AppDomain 全局异常");
            }
            catch (Exception ex)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Task 异步任务未观察到的异常（未 await 的 Task 异常）
        /// </summary>
        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                // 标记异常已处理，阻止程序崩溃
                e.SetObserved();

                // 遍历所有异常（Task 可能包含多个异常）
                foreach (var ex in e.Exception.InnerExceptions)
                {
                    ParseExceptionMsg(ex, "Task 异步任务未观察到的异常");
                }
            }
            catch (Exception ex)
            {
                Environment.Exit(1);
            }
        }

        static void ParseExceptionMsg(Exception? ex, string? backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************程序异常****************************");
            sb.AppendLine("****************************程序异常****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex?.GetType().Name);
                sb.AppendLine("【堆栈调用】：" + ex?.StackTrace);
                sb.AppendLine("【异常信息】：" + ex?.Message);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            var str = sb.ToString();
            MessageBox.Show(str, "系统异常", MessageBoxButton.OK, MessageBoxImage.Error);

            try
            {
                Log.Error(str);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "全局系统异常处理错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
