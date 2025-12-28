using PlcStepAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Events
{
    /// <summary>
    /// 弹窗事件（true表示弹窗打开了，false弹窗关闭了）
    /// </summary>
    public class DialogEvent : PubSubEvent<bool>
    {
    }
}
