using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class GlobalConfigViewModel : BaseNavigationViewModel
    {
        private string _inputStartRow = string.Empty;
        public string InputStartRow
        {
            get { return _inputStartRow; }
            set { SetProperty(ref _inputStartRow, value); }
        }

        public string InputStartCol
        {
            get { return _inputStartCol; }
            set { SetProperty(ref _inputStartCol, value); }
        }
        private string _inputStartCol = string.Empty;

        public string InputLineTime
        {
            get { return _inputLineTime; }
            set { SetProperty(ref _inputLineTime, value); }
        }
        private string _inputLineTime = string.Empty;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            InputStartRow = GlobalData.Instance.DataConfig.DefaultStartRow.ToString();
            InputStartCol = GlobalData.Instance.DataConfig.DefaultStartCol.ToString();
            InputLineTime = GlobalData.Instance.DataConfig.DefaultLineTime.ToString();
        }

        public DelegateCommand SaveCmd => _saveCmd ??= new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(InputStartRow) ||
                !int.TryParse(InputStartRow, out var row) ||
                row <= 0)
            {
                DialogManager.ShowWraningNoticeDialog("请输入正确的默认起始行数！");
                return;
            }
            if (string.IsNullOrEmpty(InputStartCol) ||
                !int.TryParse(InputStartCol, out var col) ||
                col <= 0)
            {
                DialogManager.ShowWraningNoticeDialog("请输入正确的默认起始列数！");
                return;
            }
            if (string.IsNullOrEmpty(InputLineTime) ||
                !int.TryParse(InputLineTime, out var time) ||
                time <= 0)
            {
                DialogManager.ShowWraningNoticeDialog("请输入正确的间隔时长！");
                return;
            }
            GlobalData.Instance.DataConfig.DefaultStartRow = row;
            GlobalData.Instance.DataConfig.DefaultStartCol = col;
            GlobalData.Instance.DataConfig.DefaultLineTime = time;
            var result = ConfigFileHelper.SaveConfig(GlobalData.Instance.DataConfig);
            if(result.IsSuccess)
            {
                DialogManager.ShowInfoNoticeDialog(result.Message);
            }
            else
            {
                DialogManager.ShowWraningNoticeDialog(result.Message);
            }
        });
        private DelegateCommand? _saveCmd;
    }
}
