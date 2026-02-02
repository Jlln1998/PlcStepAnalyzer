using PlcStepAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Utils
{
    public class WindowsCopyHelper
    {
        // 临时文件路径名
        public static readonly string TempFileFloder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppDatas", "Temp");

        public static OpResult CopyToTempFloder(string originFile)
        {
            var result = new OpResult();

            var targetFileName = Path.Combine(TempFileFloder, Path.GetFileName(originFile));

            if (!Directory.Exists(TempFileFloder))
            {
                Directory.CreateDirectory(TempFileFloder);
            }

            // 将文件通过 Windows 复制命令复制到临时目录，防止文件被占用无法读取。（使用Copy命令在部分环境可以绕过公司加密）
            Process process = new Process();
            string command = "copy";    // 定义要执行的命令和参数
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command} \"{originFile}\" \"{targetFileName}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 启动进程
            process.Start();

            string processOutput = process.StandardOutput.ReadToEnd();
            string processError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(processError))
            {
                result.Message = $"文件复制失败，{processOutput}\r\n{processError}";
            }
            else
            {
                if (processOutput.StartsWith("已复制") && !processOutput.Contains("0 个文件"))
                {
                    result.IsSuccess = true;
                    result.Message = $"文件复制成功，{processOutput}";
                }
                else
                {
                    result.Message = $"文件复制失败，{processOutput}\r\n{processError}";
                }
            }

            return result;
        }
    }
}
