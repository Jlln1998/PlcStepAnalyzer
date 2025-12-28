using Microsoft.Win32;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Utils;
using SqlSugar;
using System.Collections.ObjectModel;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class AnalyzerRecordItemViewModel : BaseNavigationViewModel
    {
        /// <summary>
        /// 输入的搜索名称
        /// </summary>
        public string InputName
        {
            get { return _inputName; }
            set { SetProperty(ref _inputName, value); }
        }
        private string _inputName = string.Empty;

        /// <summary>
        /// 当前查询出的解析记录项集合
        /// </summary>
        public ObservableCollection<AnalyzerRecordItem> AnalyzerRecordItems { get; set; } = [];

        /// <summary>
        /// 当前页所属的解析记录
        /// </summary>
        private AnalyzerRecord? _currentRecord;

        /// <summary>
        /// 查询命令
        /// </summary>
        public DelegateCommand QueryCmd => _queryCmd ??= new DelegateCommand(Query);
        private DelegateCommand _queryCmd;

        /// <summary>
        /// 查询命令
        /// </summary>
        public DelegateCommand<string> ExportCmd => _exportCmd ??= new DelegateCommand<string>(Export);
        private DelegateCommand<string>? _exportCmd;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var record = navigationContext.Parameters.GetValue<AnalyzerRecord>("AnalyzerRecord");
            if (record != null)
            {
                _currentRecord = record;
                Query();
            }
        }

        public void Query()
        {
            if (_currentRecord == null)
            {
                return;
            }
            AnalyzerRecordItems.Clear();
            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();

            var exp = Expressionable.Create<AnalyzerRecordItem>()
                .And(it => it.RecordId == _currentRecord.Id)
                .AndIF(!string.IsNullOrEmpty(InputName), it => it.VarName == InputName || it.StationName == InputName)
                .ToExpression();

            var list = db.Queryable<AnalyzerRecordItem>().Where(exp).ToList();
            AnalyzerRecordItems.AddRange(list);
        }

        public void Export(string type)
        {
            if (_currentRecord == null)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出流程步解析结果";
            var rang = type == "All" ? "所有" : "查询的";
            saveFileDialog.FileName = $"文件{_currentRecord.FileName}{rang}解析结果-{DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss")}";
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
                    if(type == "All")
                    {
                        var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                        var allItems = db.Queryable<AnalyzerRecordItem>().Where(it => it.RecordId == _currentRecord.Id)
                            .Select(it => new AnalyzerRecordItemVo()
                            {
                                VarName = it.VarName,
                                VarValue = it.VarValue,
                                StationName = it.StationName,
                                ActionName = it.ActionName,
                                UseTime = it.ElapsedTime.ToString("F3"),
                            })
                            .ToList();

                        var groups = allItems.GroupBy(it => it.VarName).ToList();

                        var sheets = new Dictionary<string, object> { };
                        for (int i = 0; i < groups.Count(); i++)
                        {
                            sheets.Add($"sheet{i}", groups[i].ToList());
                        }
                        MiniExcel.SaveAs(filePath, sheets, configuration: new OpenXmlConfiguration() { AutoFilter = false }, overwriteFile: true);
                    }
                    else
                    {
                        var selectedItems = AnalyzerRecordItems.Select(it => new AnalyzerRecordItemVo()
                        {
                            VarName = it.VarName,
                            VarValue = it.VarValue,
                            StationName = it.StationName,
                            ActionName = it.ActionName,
                            UseTime = it.ElapsedTime.ToString("F3"),
                        }).ToList();

                        MiniExcel.SaveAs(filePath, selectedItems, configuration: new OpenXmlConfiguration() { AutoFilter = false }, overwriteFile: true);
                    }
                    
                }
                catch (Exception ex)
                {
                    DialogManager.ShowDangerNoticeDialog($"数据导出失败：{ex.Message}");
                }
            }
        }
    }
}
