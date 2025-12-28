using PlcStepAnalyzer.Model;
using PlcStepAnalyzer.Pages.Views;
using PlcStepAnalyzer.Pages.Views.DialogPage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Config
{
    public class GlobalData:BindableBase
    {
        private static readonly GlobalData _instance = new();

        public static GlobalData Instance => _instance;

        private GlobalData() { }

        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<SysMenu> SysMenus { get; set; } =
        [
            new SysMenu { Id = 1, Pid = 0, Name = "文件解析", IconKey = "FloderIcon", Selected = false},
            new SysMenu { Id = 2, Pid = 1, Name = "解析记录", ViewName=nameof(AnalyzerRecordView), Selected = false },
            new SysMenu { Id = 3, Pid = 1, Name = "解析结果", ViewName=nameof(AnalyzerRecordItemView), Selected = false },
            
            new SysMenu { Id = 4, Pid = 0, Name = "配置方案", IconKey = "ConfigIcon", Selected = false},
            new SysMenu { Id = 5, Pid = 4, Name = "方案列表", ViewName=nameof(VarConfigListView), Selected = false },
            new SysMenu { Id = 6, Pid = 4, Name = "方案详情", ViewName=nameof(VarConfigItemListView), Selected = false },
            
            new SysMenu { Id = 7, Pid = 0, Name = "软件设置", IconKey = "SettingsIcon", Selected = false },
            new SysMenu { Id = 8, Pid = 7, Name = "全局设置", ViewName=nameof(GlobalConfigView), Selected = false },
            
            new SysMenu { Id = 9, Pid = 0, Name = "关于软件", IconKey = "AboutIcon", Selected = false,Additional = true },
            new SysMenu { Id = 10, Pid = 9, Name = "软件介绍",ViewName=nameof(AboutView), Selected = false,Additional = true }
        ];

        public DataConfig DataConfig { get; set; } = new DataConfig();
    }
}
