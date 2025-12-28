using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class ShowProgressBarViewModel : BindableBase
    {
        /// <summary>
        /// 进度值
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
        private int _value;

        /// <summary>
        /// 进度描述文本
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }
        private string _Text = string.Empty;

        /// <summary>
        /// 设置进度值和进度文本
        /// </summary>
        /// <param name="value">进度值</param>
        /// <param name="message">进度描述文本</param>
        /// <returns></returns>
        public async Task SetProcessStatus(int value, string message)
        {
            Text = message;
            for (int i = Value; i < value; i++)
            {
                await Task.Delay(12);
                Value = i;
                if(i == 100)
                {
                    Text = "解析完毕！";
                    await Task.Delay(1000);
                }
            }
        }
    }
}
