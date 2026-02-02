using Mapster;
using Microsoft.Win32;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Pages.Views.DialogPage;
using PlcStepAnalyzer.Utils;
using SqlSugar;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Shapes;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class VarConfigItemListViewModel : BaseNavigationViewModel
    {
        /// <summary>
        /// 传入的配置表信息
        /// </summary>
        public VarConfigVo? VarConfigVo
        {
            get { return _varConfigVo; }
            set { SetProperty(ref _varConfigVo, value); }
        }
        private VarConfigVo? _varConfigVo;

        /// <summary>
        /// 输入的配置项信息
        /// </summary>
        private VarConfigItem _inputConfigItem = new VarConfigItem();
        public VarConfigItem InputConfigItem
        {
            get { return _inputConfigItem; }
            set { SetProperty(ref _inputConfigItem, value); }
        }

        /// <summary>
        /// 查询出的配置项列表
        /// </summary>
        public ObservableCollection<VarConfigItem> QueriedConfigItems { get; set; } = [];

        /// <summary>
        /// 编辑配置表名命令
        /// </summary>
        public DelegateCommand EditConfigNameCmd => _editConfigNameCmd ??= new DelegateCommand(EditConfigName);
        private DelegateCommand? _editConfigNameCmd;

        /// <summary>
        /// 查询命令
        /// </summary>
        public DelegateCommand QueryCmd => _queryCmd ??= new DelegateCommand(Query);
        private DelegateCommand? _queryCmd;

        /// <summary>
        /// 新增或者修改配置项命令
        /// </summary>
        public DelegateCommand<VarConfigItem> AddOrUpdateCmd => _addOrUpdateCmd ??= new DelegateCommand<VarConfigItem>(AddOrUpdate);
        private DelegateCommand<VarConfigItem>? _addOrUpdateCmd;

        /// <summary>
        /// 删除命令
        /// </summary>
        public DelegateCommand<VarConfigItem> DeleteCmd => _deletelCmd ??= new DelegateCommand<VarConfigItem>(Delete);
        private DelegateCommand<VarConfigItem>? _deletelCmd;

        /// <summary>
        /// 清空命令
        /// </summary>
        public DelegateCommand ClearAllCmd => _clearAllCmd ??= new DelegateCommand(ClearAll);
        private DelegateCommand? _clearAllCmd;

        /// <summary>
        /// 导出模板命令
        /// </summary>
        public DelegateCommand ExportTemplateCmd => _exportTemplateCmd ??= new DelegateCommand(ExportTemplate);
        private DelegateCommand? _exportTemplateCmd;

        /// <summary>
        /// 导出数据
        /// </summary>
        public DelegateCommand<string> ExportDataCmd => _exportDataCmd ??= new DelegateCommand<string>(ExportData);
        private DelegateCommand<string>? _exportDataCmd;

        /// <summary>
        /// 导入输入
        /// </summary>
        private DelegateCommand? _importDataCmd;
        public DelegateCommand ImportDataCmd => _importDataCmd ??= new DelegateCommand(ImportData);


        public VarConfigItemListViewModel()
        {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            VarConfigVo = navigationContext.Parameters.GetValue<VarConfigVo>("VarConfigVo");
            InputConfigItem.ConfigId = VarConfigVo.Id;
            Query();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            QueriedConfigItems.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void Query()
        {
            if (VarConfigVo == null)
            {
                return;
            }

            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();

            var exp = Expressionable.Create<VarConfigItem>();
            exp.And(it => it.ConfigId == InputConfigItem.ConfigId);
            exp.AndIF(!string.IsNullOrEmpty(InputConfigItem.VarName), it => it.VarName.Contains(InputConfigItem.VarName));
            exp.AndIF(!string.IsNullOrEmpty(InputConfigItem.VarValue), it => it.VarValue == InputConfigItem.VarValue);
            exp.AndIF(!string.IsNullOrEmpty(InputConfigItem.StationName), it => it.StationName.Contains(InputConfigItem.StationName));
            exp.AndIF(!string.IsNullOrEmpty(InputConfigItem.ActionName), it => it.ActionName.Contains(InputConfigItem.ActionName));

            QueriedConfigItems.Clear();
            var details = db.Queryable<VarConfigItem>().Where(exp.ToExpression()).ToList();
            QueriedConfigItems.AddRange(details);
        }

        private void EditConfigName()
        {
            if (VarConfigVo == null)
            {
                return;
            }

            var dialogService = ContainerLocator.Container.Resolve<IDialogService>();
            var parameters = new DialogParameters
            {
                { "VarConfigVo", VarConfigVo}
            };
            dialogService.ShowDialog(nameof(EditVarConfigNameView), parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    VarConfigVo.ConfigName = db.Queryable<VarConfig>().First(it => it.Id == VarConfigVo.Id).Name;
                }
            });
        }

        private void AddOrUpdate(VarConfigItem? varConfigItem)
        {
            if (VarConfigVo == null)
            {
                return;
            }

            var dialogService = ContainerLocator.Container.Resolve<IDialogService>();

            var parameters = new DialogParameters();

            var oldVarConfigItem = varConfigItem == null ? new VarConfigItem() { ConfigId = VarConfigVo.Id } : varConfigItem.Adapt<VarConfigItem>();
            parameters.Add("OldVarConfigItem", oldVarConfigItem);

            dialogService.ShowDialog(nameof(AddOrUpdateConfigItemDialogView), parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    Query();
                }
            });
        }

        private void Delete(VarConfigItem? paramsMapDetail)
        {
            if (paramsMapDetail == null)
            {
                return;
            }
            DialogManager.ShowWraningConfirmDialog($"是否要删除该配置项:\r\n[{paramsMapDetail.StationName}]-[{paramsMapDetail.ActionName}]？", (result) =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    db.Deleteable<VarConfigItem>().Where(it => it.Id == paramsMapDetail.Id).ExecuteCommand();
                    Query();
                }
            });
        }

        private void ClearAll()
        {
            if (VarConfigVo == null)
            {
                return;
            }
            DialogManager.ShowDangerConfirmDialog($"是否要清空:配置表 [{VarConfigVo.ConfigName}] 中的所有参数配置？", (result) =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    db.Deleteable<VarConfigItem>().Where(it => it.ConfigId == VarConfigVo.Id).ExecuteCommand();
                    Query();
                }
            });
        }

        private void ExportTemplate()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "保存配置项模板";
            saveFileDialog.FileName = "参数配置项模板";
            saveFileDialog.Filter = "Excel文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    MiniExcel.SaveAs(filePath, new VarConfigItemVo[0], configuration: new OpenXmlConfiguration() { AutoFilter = false }, overwriteFile: true);
                }
                catch (Exception ex)
                {
                    DialogManager.ShowDangerNoticeDialog($"模板导出失败：{ex.Message}");
                }
            }
        }

        private void ExportData(string model)
        {
            if (VarConfigVo == null)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出配置表";
            saveFileDialog.FileName = $"配置表{VarConfigVo.ConfigName}-{DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss")}数据";
            saveFileDialog.Filter = "Excel文件 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    List<VarConfigItemVo> items = [];
                    if (model == "All")
                    {
                        items = db.Queryable<VarConfigItem>().Where(it => it.ConfigId == VarConfigVo.Id)
                        .Select(it => new VarConfigItemVo()
                        {
                            VarName = it.VarName,
                            VarValue = it.VarValue,
                            StationName = it.StationName,
                            ActionName = it.ActionName,
                        })
                        .ToList();
                    }
                    else
                    {
                        items = QueriedConfigItems
                            .Select(it => new VarConfigItemVo()
                            {
                                VarName = it.VarName,
                                VarValue = it.VarValue,
                                StationName = it.StationName,
                                ActionName = it.ActionName,
                            })
                        .ToList();
                    }
                    MiniExcel.SaveAs(filePath, items, configuration: new OpenXmlConfiguration() { AutoFilter = false }, overwriteFile: true);
                }
                catch (Exception ex)
                {
                    DialogManager.ShowDangerNoticeDialog($"数据导出失败：{ex.Message}");
                }
            }
        }

        private void ImportData()
        {
            if (VarConfigVo == null)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择要导入的配置文件";
            openFileDialog.Filter = "Excel 文件 (*.xlsx;*.xls)|*.xlsx;*.xls|所有文件 (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.CheckPathExists = true;

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    using (var stream = File.OpenRead(filePath))
                    {
                        var changed = false;
                        var rows = stream.Query<VarConfigItemVo>();
                        foreach (var detail in rows)
                        {
                            // 如果遇到 ConfigId、 params 和 value 一致的数据
                            var data = db.Queryable<VarConfigItem>()
                                            .First(it => it.ConfigId == VarConfigVo.Id &&
                                                it.VarName == detail.VarName &&
                                                it.VarValue == detail.VarValue);
                            if (data != null)
                            {
                                changed = true;
                                data.StationName = detail.StationName;
                                data.ActionName = detail.ActionName;
                                db.Updateable<VarConfigItem>(data).ExecuteCommand();
                                continue;
                            }
                            var insertData = new VarConfigItem();
                            insertData.Id = Guid.NewGuid();
                            insertData.ConfigId = VarConfigVo.Id;
                            insertData.VarName = detail.VarName;
                            insertData.VarValue = detail.VarValue;
                            insertData.StationName = detail.StationName;
                            insertData.ActionName = detail.ActionName;
                            db.Insertable<VarConfigItem>(insertData).ExecuteCommand();
                            changed = true;
                        }
                        if (changed)
                        {
                            db.Updateable<VarConfig>()
                            .SetColumns(it => new VarConfig()
                            {
                                UpdateTime = DateTime.Now,
                            })
                            .Where(it => it.Id == VarConfigVo.Id)
                            .ExecuteCommand();
                        }
                        Query();
                    }
                }
                catch (Exception ex)
                {
                    DialogManager.ShowDangerNoticeDialog($"数据导入失败：{ex.Message}");
                }
            }
        }
    }
}
