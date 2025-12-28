namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 文件解析记录
    /// </summary>
    public class AnalyzerRecord : DbEntity
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 数据启始行
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// 数据起始列
        /// </summary>
        public int StartCol { get; set; }

        /// <summary>
        /// 每行数据间隔时间，单位毫秒
        /// </summary>
        public int IndexTime { get; set; }

        /// <summary>
        /// 采用的配置表Id
        /// </summary>
        public Guid VarConfigId { get; set; }

        /// <summary>
        /// 采用的配置表名称
        /// </summary>
        public string VarConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置表是否已经被删除
        /// </summary>
        public bool VarConfigIsDelete { get; set; }
    }
}
