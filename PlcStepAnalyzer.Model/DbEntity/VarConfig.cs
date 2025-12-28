using SqlSugar;

namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 变量配置
    /// </summary>
    public class VarConfig : DbEntity
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 配置中的所有子项
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(VarConfigItem.ConfigId))]
        public List<VarConfigItem> Items { get; set; }
    }
}
