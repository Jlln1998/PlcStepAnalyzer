using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Model
{
    /// <summary>
    /// 菜单对象
    /// </summary>
    public class SysMenu : BindableBase
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 父菜单Id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name = string.Empty;

        /// <summary>
        /// 图标 Key 值
        /// </summary>
        public string IconKey
        {
            get { return _iconKey; }
            set { SetProperty(ref _iconKey, value); }
        }
        private string _iconKey = string.Empty;

        /// <summary>
        /// 导航对应的页面视图名称
        /// </summary>
        public string ViewName { get; set; } = string.Empty;

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        private bool _selected;

        /// <summary>
        /// 是否是一些特殊附加页，如“关于软件”之类的页面
        /// </summary>
        public bool Additional
        {
            get { return _additional; }
            set { SetProperty(ref _additional, value); }
        }
        private bool _additional;
    }
}
