using PlcStepAnalyzer.Pages.Views.DialogPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PlcStepAnalyzer.Utils
{
    public static class DialogManager
    {
        // 提示框
        public static void ShowInfoNoticeDialog(string message, string title = "提示", Action<IDialogResult>? callback = null)
        {
            var brush = Application.Current.TryFindResource("SubjectColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Collapsed, brush ?? Brushes.LightBlue, callback);
        }
        public static void ShowWraningNoticeDialog(string message, string title = "提示", Action<IDialogResult>? callback = null)
        {
            var brush = Application.Current.TryFindResource("WraningColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Collapsed, brush ?? Brushes.LightBlue, callback);
        }
        public static void ShowDangerNoticeDialog(string message, string title = "提示", Action<IDialogResult>? callback = null)
        {
            var brush = Application.Current.TryFindResource("DangerColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Collapsed, brush ?? Brushes.LightBlue, callback);
        }
        // 确认框
        public static void ShowInfoConfirmDialog(string message, Action<IDialogResult>? callback = null, string title = "确认操作")
        {
            var brush = Application.Current.TryFindResource("SubjectColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Visible, brush ?? Brushes.LightBlue, callback);
        }
        public static void ShowWraningConfirmDialog(string message, Action<IDialogResult>? callback = null, string title = "确认操作")
        {
            var brush = Application.Current.TryFindResource("WraningColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Visible, brush ?? Brushes.LightBlue, callback);
        }
        public static void ShowDangerConfirmDialog(string message, Action<IDialogResult>? callback = null, string title = "确认操作")
        {
            var brush = Application.Current.TryFindResource("DangerColor") as SolidColorBrush;
            ShowConfirmDialog(title, message, Visibility.Visible, brush ?? Brushes.LightBlue, callback);
        }

        private static void ShowConfirmDialog(string title, string message, Visibility cancelBtnVisibility, Brush typeBrush, Action<IDialogResult>? callback)
        {
            var dialogService = ContainerLocator.Container.Resolve<IDialogService>();
            DialogParameters parameters = new DialogParameters();
            parameters.Add("Message", message);
            parameters.Add("CancelBtnVisibility", cancelBtnVisibility);
            parameters.Add("TypeBrush", typeBrush);
            parameters.Add("Title", title);
            dialogService.ShowDialog(nameof(NoticeDialogView), parameters, callback);
        }
    }
}
