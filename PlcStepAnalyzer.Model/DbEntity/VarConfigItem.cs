namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 变量配置项
    /// </summary>
    public class VarConfigItem : DbEntity
    {
        /// <summary>
        /// 变量配置Id
        /// </summary>
        public Guid ConfigId { get; set; }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string VarName { get; set; } = string.Empty;

        /// <summary>
        /// 变量值
        /// </summary>
        public string VarValue { get; set; } = string.Empty;

        /// <summary>
        /// 对应工位名
        /// </summary>
        public string StationName { get; set; } = string.Empty;

        /// <summary>
        /// 对应动作名
        /// </summary>
        public string ActionName { get; set; } = string.Empty;
    }
}
