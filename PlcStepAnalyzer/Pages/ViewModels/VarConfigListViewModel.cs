using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Events;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Pages.Views.DialogPage;
using PlcStepAnalyzer.Utils;
using SqlSugar;
using System.Collections.ObjectModel;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class VarConfigListViewModel : BindableBase
    {
        /// <summary>
        /// 配置表列表
        /// </summary>
        public ObservableCollection<VarConfigVo> Configs { get; set; } = [];

        /// <summary>
        /// 输入的查询配置表名称
        /// </summary>
        public string InputConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 查询命令
        /// </summary>
        public DelegateCommand<string> QueryCmd => _queryCmd ??= new DelegateCommand<string>(Query);
        private DelegateCommand<string>? _queryCmd;

        /// <summary>
        /// 新增命令
        /// </summary>
        public DelegateCommand AddCmd => _addCmd ??= new DelegateCommand(Add);
        private DelegateCommand? _addCmd;

        /// <summary>
        /// 编辑命令
        /// </summary>
        public DelegateCommand<VarConfigVo> EditCmd => _editCmd ??= new DelegateCommand<VarConfigVo>(Edit);
        private DelegateCommand<VarConfigVo>? _editCmd;

        /// <summary>
        /// 删除命令
        /// </summary>
        public DelegateCommand<VarConfigVo> DeleteCmd => _deleteCmd ??= new DelegateCommand<VarConfigVo>(Delete);
        private DelegateCommand<VarConfigVo>? _deleteCmd;


        public VarConfigListViewModel()
        {
            Query();
        }

        /// <summary>
        /// 查询配置表
        /// </summary>
        private void Query(string name = "")
        {
            Configs.Clear();

            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
            var configs = db.Queryable<VarConfig>()
                .WhereIF(!string.IsNullOrEmpty(name), it => it.Name.Contains(name))
                .Includes(it => it.Items)
                .ToList();

            var configVoList = new List<VarConfigVo>();
            foreach (var config in configs)
            {
                configVoList.Add(new VarConfigVo()
                {
                    Id = config.Id,
                    ConfigName = config.Name,
                    VarQty = config.Items?.Count ?? 0,
                    CreateTime = config.CreateTime,
                    UpdateTime = config.UpdateTime,
                });
            }

            Configs.AddRange(configVoList);
        }

        private void Add()
        {
            var dialogService = ContainerLocator.Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(AddVarConfigDialogView), null, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    Query(InputConfigName);
                }
            });
        }

        private void Edit(VarConfigVo configVo)
        {
            var eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();

            var sysMenu = GlobalData.Instance.SysMenus.FirstOrDefault(it => it.Id == 6);
            var navParams = new NavigationParameters()
            {
                {"VarConfigVo",configVo}
            };
            eventAggregator.GetEvent<NavToMenuEvent>().Publish((sysMenu, navParams));
        }

        private void Delete(VarConfigVo paramsMap)
        {
            DialogManager.ShowWraningConfirmDialog($"是否要删除配置表:【{paramsMap.ConfigName}】？", (result) =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    // 先删配置项
                    db.Deleteable<VarConfigItem>().Where(it => it.ConfigId == paramsMap.Id).ExecuteCommand();
                    // 再删配置表
                    db.Deleteable<VarConfig>().Where(it => it.Id == paramsMap.Id).ExecuteCommand();
                    // 更新解析记录表中的引用该配置表的记录，将配置表标记为已删除
                    db.Updateable<AnalyzerRecord>()
                        .SetColumns(it => new AnalyzerRecord() { VarConfigIsDelete = true })
                        .Where(it => it.VarConfigId == paramsMap.Id)
                        .ExecuteCommand();
                    // 刷新列表
                    Query(InputConfigName);
                }
            });
        }
    }
}
