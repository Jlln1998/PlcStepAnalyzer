using Microsoft.Win32;
using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Model.DbEntity;
using Prism.Ioc;
using SqlSugar;
using System.Collections.ObjectModel;
using System.Windows;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class NewAnalyzerRecordViewModel : BaseDialogViewModel
    {
        /// <summary>
        /// 选择的文件全路径名
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    SelectedFileVisibility = Visibility.Visible;
                }
            }
        }
        public string _fileName = string.Empty;

        /// <summary>
        /// 选择的文件名文本是否可见
        /// </summary>
        public Visibility SelectedFileVisibility
        {
            get { return _SelectedFileVisibility; }
            set { SetProperty(ref _SelectedFileVisibility, value); }
        }
        private Visibility _SelectedFileVisibility = Visibility.Collapsed;

        /// <summary>
        /// 选择的文件路径
        /// </summary>
        public string SelectFilePath
        {
            get { return _selectFilePath; }
            set { SetProperty(ref _selectFilePath, value); }
        }
        private string _selectFilePath = string.Empty;

        /// <summary>
        /// 选择的文件名
        /// </summary>
        public string SelectFileName
        {
            get { return _selectFileName; }
            set { SetProperty(ref _selectFileName, value); }
        }
        private string _selectFileName = string.Empty;

        /// <summary>
        /// 变量配置表
        /// </summary>
        public ObservableCollection<VarConfig> VarConfigs { get; set; } = [];

        /// <summary>
        /// 选中的变量配置
        /// </summary>
        public VarConfig? SelectedVarConfig
        {
            get { return _selectedVarConfig; }
            set { SetProperty(ref _selectedVarConfig, value); }
        }
        private VarConfig? _selectedVarConfig;

        /// <summary>
        /// 输入的起始行
        /// </summary>
        public string InputRow
        {
            get { return _inputRow; }
            set { SetProperty(ref _inputRow, value); }
        }
        private string _inputRow = string.Empty;

        /// <summary>
        /// 输入的起始列
        /// </summary>
        public string InputCol
        {
            get { return _inputCol; }
            set { SetProperty(ref _inputCol, value); }
        }
        private string _inputCol = string.Empty;

        /// <summary>
        /// 输入的起始时间
        /// </summary>
        public string InputIndexTime
        {
            get { return _inputIndexTime; }
            set { SetProperty(ref _inputIndexTime, value); }
        }
        private string _inputIndexTime = string.Empty;

        /// <summary>
        /// 文件选择命令
        /// </summary>
        public DelegateCommand SelectFileCmd => _selectFileCmd ??= new DelegateCommand(SelectFile);
        private DelegateCommand? _selectFileCmd;

        /// <summary>
        /// 关闭命令
        /// </summary>
        public DelegateCommand<string> CloseCmd => _closeCmd ??= new DelegateCommand<string>(OnClose);
        private DelegateCommand<string>? _closeCmd;

        public NewAnalyzerRecordViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            InputRow = GlobalData.Instance.DataConfig.DefaultStartRow.ToString();
            InputCol = GlobalData.Instance.DataConfig.DefaultStartCol.ToString();
            InputIndexTime = GlobalData.Instance.DataConfig.DefaultLineTime.ToString();
            Query();
        }

        private void Query(string configName = "")
        {
            VarConfigs.Clear();

            var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
            var allConfigs = db.Queryable<VarConfig>().ToList();
            VarConfigs.AddRange(allConfigs);
        }

        private void SelectFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*";
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                FileName = openFileDialog.FileName;
                SelectFilePath = System.IO.Path.GetDirectoryName(openFileDialog.FileName) + "\\" ?? string.Empty;
                SelectFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void OnClose(string type)
        {
            if (type == "Confirm")
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    ErrorMsg = "请选择要解析的文件!";
                    return;
                }
                if (SelectedVarConfig == null)
                {
                    ErrorMsg = "请选择要使用的配置文件!";
                    return;
                }
                if (string.IsNullOrEmpty(InputRow) || !int.TryParse(InputRow, out var inputRow) || inputRow <= 0)
                {
                    ErrorMsg = "输入的起始行错误!数值应该 >= 1";
                    return;
                }
                if (string.IsNullOrEmpty(InputCol) || !int.TryParse(InputCol, out var inputCol) || inputCol <= 0)
                {
                    ErrorMsg = "输入的起始列错误!数值应该 >= 1";
                    return;
                }
                if (string.IsNullOrEmpty(InputIndexTime) || !int.TryParse(InputIndexTime, out var inputTime) || inputTime <= 0)
                {
                    ErrorMsg = "输入的间隔时间错误!数值应该 >= 1";
                    return;
                }

                var analyzerRecord = new AnalyzerRecord()
                {
                    Id = Guid.NewGuid(),
                    FilePath = SelectFilePath,
                    FileName = SelectFileName,
                    StartRow = inputRow,
                    StartCol = inputCol,
                    IndexTime = inputTime,
                    VarConfigId = SelectedVarConfig.Id,
                    VarConfigName = SelectedVarConfig.Name
                };

                var dialogParams = new DialogParameters()
                {
                    {"AnalyzerRecord",analyzerRecord}
                };
                RequestClose.Invoke(dialogParams, ButtonResult.OK);
            }
            else
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
        }
    }
}
