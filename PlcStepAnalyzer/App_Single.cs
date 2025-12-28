using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer
{
    public partial class App
    {
        // 1. 定义互斥锁（全局唯一名称，建议用公司/程序唯一标识）
        private static Mutex _mutex;
        // 确保互斥锁是全局的（跨会话生效）
        private const string UniqueMutexName = "{B330C481-616F-4EEC-8660-48F63842C20C}";

        // 2. 导入Win32 API（用于查找窗口、置顶激活）
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // ShowWindow 常量：恢复窗口并激活
        private const int SW_RESTORE = 9;

        public bool CheckSingle()
        {
            bool createdNew;

            _mutex = new Mutex(true, UniqueMutexName, out createdNew);

            if (!createdNew)
            {
                BringRunningInstanceToFront();
                return false;
            }

            return createdNew;
        }

        /// <summary>
        /// 查找已运行的实例窗口并置顶显示
        /// </summary>
        private void BringRunningInstanceToFront()
        {
            IntPtr hWnd = IntPtr.Zero;

            if (hWnd == IntPtr.Zero)
            {
                Process currentProcess = Process.GetCurrentProcess();

                foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))
                {
                    if (process.Id != currentProcess.Id)
                    {
                        hWnd = process.MainWindowHandle;
                        break;
                    }
                }
            }

            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
        }
    }
}
