using PlcStepAnalyzer.Events;
using System.Windows;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    /// <summary>
    /// 弹窗视图模型基类
    /// </summary>
    public class BaseDialogViewModel(IContainerProvider containerProvider) : BindableBase, IDialogAware
    {
        public DialogCloseListener RequestClose { get; set; }

        protected IEventAggregator _eventAggregator = containerProvider.Resolve<IEventAggregator>();

        /// <summary>
        /// 弹窗标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title = "";

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set
            {
                SetProperty(ref _errorMsg, value);
                ErrorMsgVisibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private string _errorMsg = string.Empty;

        /// <summary>
        /// 错误信息文本可见性
        /// </summary>
        public Visibility ErrorMsgVisibility
        {
            get { return _errorMsgVisibility; }
            set { SetProperty(ref _errorMsgVisibility, value); }
        }
        private Visibility _errorMsgVisibility = Visibility.Collapsed;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(false);
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            var title = parameters.GetValue<string>("Title");

            Title = string.IsNullOrEmpty(title) ? "弹窗" : title;

            _eventAggregator.GetEvent<DialogEvent>().Publish(true);
        }
    }
}
