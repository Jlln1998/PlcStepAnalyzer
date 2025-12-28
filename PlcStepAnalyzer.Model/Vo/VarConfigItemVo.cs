using MiniExcelLibs.Attributes;

namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 变量配置项视图对象
    /// </summary>
    public class VarConfigItemVo : BindableBase
    {
        /// <summary>
        /// 变量配置Id
        /// </summary>
        [ExcelIgnore]
        public string ConfigId
        {
            get { return _configId; }
            set { SetProperty(ref _configId, value); }
        }
        private string _configId = string.Empty;

        /// <summary>
        /// 变量名
        /// </summary>
        [ExcelColumnName("变量名"), ExcelColumnWidth(70)]
        public string VarName
        {
            get { return _varName; }
            set { SetProperty(ref _varName, value); }
        }
        private string _varName = string.Empty;

        /// <summary>
        /// 变量值
        /// </summary>
        [ExcelColumnName("变量值"), ExcelColumnWidth(15)]
        public string VarValue
        {
            get { return _varValue; }
            set { SetProperty(ref _varValue, value); }
        }
        private string _varValue = string.Empty;

        /// <summary>
        /// 工位名
        /// </summary>
        [ExcelColumnName("工位名"), ExcelColumnWidth(22)]
        public string StationName
        {
            get { return _stationName; }
            set { SetProperty(ref _stationName, value); }
        }
        private string _stationName = string.Empty;

        /// <summary>
        /// 对应动作名称
        /// </summary>
        [ExcelColumnName("动作名"), ExcelColumnWidth(22)]
        public string ActionName
        {
            get { return _actionName; }
            set { SetProperty(ref _actionName, value); }
        }
        private string _actionName = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelIgnore]
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { SetProperty(ref _createTime, value); }
        }
        private DateTime _createTime;

        /// <summary>
        /// 最新修改时间
        /// </summary>
        [ExcelIgnore]
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { SetProperty(ref _updateTime, value); }
        }
        private DateTime _updateTime;
    }
}
