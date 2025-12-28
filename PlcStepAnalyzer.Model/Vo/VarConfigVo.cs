namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 变量配置视图对象
    /// </summary>
    public class VarConfigVo : BindableBase
    {
        /// <summary>
        /// 变量配置Id
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private Guid _id;

        /// <summary>
        /// 变量配置名称
        /// </summary>
        public string ConfigName
        {
            get { return _configName; }
            set { SetProperty(ref _configName, value); }
        }
        private string _configName = string.Empty;

        /// <summary>
        /// 配置变量数
        /// </summary>
        public int VarQty
        {
            get { return _varQty; }
            set { SetProperty(ref _varQty, value); }
        }
        private int _varQty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { SetProperty(ref _createTime, value); }
        }
        private DateTime _createTime;

        /// <summary>
        /// 最新修改时间
        /// </summary>
        private DateTime _updateTime;
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { SetProperty(ref _updateTime, value); }
        }
    }
}
