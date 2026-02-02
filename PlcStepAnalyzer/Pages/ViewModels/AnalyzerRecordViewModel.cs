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
    public class AnalyzerRecordViewModel : BindableBase
    {
        /// <summary>
        /// 文件解析记录
        /// </summary>
        public ObservableCollection<AnalyzerRecord> AnalyzerRecords { get; set; } = [];

        /// <summary>
        /// 新建解析命令
        /// </summary>
        public DelegateCommand NewAnalyzerRecordCmd => _newAnalyzerRecordCmd ??= new DelegateCommand(NewAnalyzer);
        private DelegateCommand? _newAnalyzerRecordCmd;

        /// <summary>
        /// 查看解析记录详情命令
        /// </summary>
        public DelegateCommand<AnalyzerRecord?> WatchDetailCmd => _watchDetailCmd ??= new DelegateCommand<AnalyzerRecord?>(WatchDetail);
        private DelegateCommand<AnalyzerRecord?>? _watchDetailCmd;

        /// <summary>
        /// 删除命令
        /// </summary>
        public DelegateCommand<AnalyzerRecord?> DeleteCmd => _deleteCmd ??= new DelegateCommand<AnalyzerRecord?>(Delete);
        private DelegateCommand<AnalyzerRecord?>? _deleteCmd;

        public AnalyzerRecordViewModel()
        {
            Query();
        }

        private void Query()
        {
            AnalyzerRecords.Clear();

            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
            var allfiles = db.Queryable<AnalyzerRecord>().ToList();
            AnalyzerRecords.AddRange(allfiles);
        }

        private async void NewAnalyzer()
        {
            // 弹窗
            AnalyzerRecord? analyzerRecord = null;
            var dialogService = ContainerLocator.Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(NewAnalyzerRecordView), new DialogParameters()
            {
                { "Title", "新建解析" }
            },
            result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    analyzerRecord = result.Parameters.GetValue<AnalyzerRecord>("AnalyzerRecord");
                }
            });

            if (analyzerRecord == null)
            {
                return;
            }

            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
            db.Insertable(analyzerRecord).ExecuteCommand();

            var result = await StepAnalyzer.Analyzer(analyzerRecord);
            if (result.IsSuccess)
            {
                WatchDetail(analyzerRecord);
            }
            else
            {
                db.Deleteable(analyzerRecord).ExecuteCommand();
                Query();
                DialogManager.ShowWraningNoticeDialog(result.Message);
            }
        }

        private void WatchDetail(AnalyzerRecord? entity)
        {
            if (entity == null)
            {
                return;
            }

            var eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();

            var sysMenu = GlobalData.Instance.SysMenus.FirstOrDefault(it => it.Id == 3);
            var navParams = new NavigationParameters()
            {
                {"AnalyzerRecord",entity}
            };
            eventAggregator.GetEvent<NavToMenuEvent>().Publish((sysMenu, navParams));
        }

        private void Delete(AnalyzerRecord? entity)
        {
            if (entity == null)
            {
                return;
            }
            DialogManager.ShowWraningConfirmDialog($"是否要删除解析记录:\r\n[{entity.FileName}]？", (result) =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    // 先删Item
                    db.Deleteable<AnalyzerRecordItem>().Where(it => it.RecordId == entity.Id).ExecuteCommand();
                    // 再删除record
                    db.Deleteable<AnalyzerRecord>().Where(it => it.Id == entity.Id).ExecuteCommand();
                    // 刷新数据
                    Query();
                }
            });
        }
    }
}
