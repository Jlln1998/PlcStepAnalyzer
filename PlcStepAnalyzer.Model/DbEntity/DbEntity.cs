using SqlSugar;

namespace PlcStepAnalyzer.Model.DbEntity
{
    /// <summary>
    /// 数据库实体基类
    /// </summary>
    public class DbEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(InsertServerTime = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最新修改时间
        /// </summary>
        [SugarColumn(InsertServerTime = true, UpdateServerTime = true)]
        public DateTime UpdateTime { get; set; }
    }
}
