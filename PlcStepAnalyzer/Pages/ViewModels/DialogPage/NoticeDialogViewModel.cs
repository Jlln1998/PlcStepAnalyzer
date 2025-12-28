using Azure;
using PlcStepAnalyzer.Events;
using PlcStepAnalyzer.Model;
using PlcStepAnalyzer.Model.DbEntity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class NoticeDialogViewModel(IContainerProvider containerProvider) : BaseDialogViewModel(containerProvider)
    {
        /// <summary>
        /// 通知消息内容
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        private string _message = string.Empty;

        /// <summary>
        /// 通知显示色彩
        /// </summary>
        public Brush TypeBrush
        {
            get { return _typeBrush; }
            set { SetProperty(ref _typeBrush, value); }
        }
        private Brush _typeBrush = Brushes.LightBlue;

        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        public Visibility CancelBtnVisibility
        {
            get { return _cancelBtnVisibility; }
            set { SetProperty(ref _cancelBtnVisibility, value); }
        }
        private Visibility _cancelBtnVisibility = Visibility.Visible;

        /// <summary>
        /// 弹窗关闭命令
        /// </summary>
        public DelegateCommand<string> CloseCmd => _closeCmd ??= new DelegateCommand<string>(OnClose);
        private DelegateCommand<string>? _closeCmd;

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            Message = parameters.GetValue<string>("Message");
            CancelBtnVisibility = parameters.GetValue<Visibility>("CancelBtnVisibility");
            TypeBrush = parameters.GetValue<Brush>("TypeBrush");
        }

        private void OnClose(string type)
        {
            if (type == "Confirm")
            {
                RequestClose.Invoke(ButtonResult.OK);
            }
            else
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
        }
    }
}
