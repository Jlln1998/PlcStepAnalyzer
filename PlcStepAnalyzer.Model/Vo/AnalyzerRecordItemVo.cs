using MiniExcelLibs.Attributes;

namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 流步骤解析结果视图类
    /// </summary>
    public class AnalyzerRecordItemVo
    {
        /// <summary>
        /// 变量名
        /// </summary>
        [ExcelColumnName("变量名"), ExcelColumnWidth(50)]
        public string VarName { get; set; } = string.Empty;
        /// <summary>
        /// 变量配置Id
        /// </summary>
        [ExcelColumnName("工位名"), ExcelColumnWidth(50)]
        public string StationName { get; set; } = string.Empty;
        /// <summary>
        /// 变量值
        /// </summary>
        [ExcelColumnName("变量值"), ExcelColumnWidth(30)]
        public string VarValue { get; set; } = string.Empty;
        /// <summary>
        /// 动作名称
        /// </summary>
        [ExcelColumnName("动作名"), ExcelColumnWidth(30)]
        public string ActionName { get; set; } = string.Empty;
        /// <summary>
        /// 动作耗时
        /// </summary>
        [ExcelColumnName("耗时(s)"), ExcelColumnWidth(30)]
        public string UseTime { get; set; } = string.Empty;
    }
}
