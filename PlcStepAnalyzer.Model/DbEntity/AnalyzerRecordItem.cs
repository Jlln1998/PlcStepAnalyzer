namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 文件解析记录项
    /// </summary>
    public class AnalyzerRecordItem : DbEntity
    {
        /// <summary>
        /// 文件解析记录Id
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>
        /// 变量名
        /// </summary>
        public string VarName { get; set; } = string.Empty;

        /// <summary>
        /// 变量值
        /// </summary>
        public string VarValue { get; set; } = string.Empty;

        /// <summary>
        /// 工位名
        /// </summary>
        public string StationName { get; set; } = string.Empty;

        /// <summary>
        /// 步骤名
        /// </summary>
        public string ActionName { get; set; } = string.Empty;

        /// <summary>
        /// 动作耗时（s）
        /// </summary>
        public double ElapsedTime { get; set; }
    }
}
