using PlcStepAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Events
{
    /// <summary>
    /// 导航到菜单事件
    /// </summary>
    public class NavToMenuEvent : PubSubEvent<(SysMenu? TargetMenu, NavigationParameters? NavParams)>
    {
    }
}
