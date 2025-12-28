using PlcStepAnalyzer.Events;
using PlcStepAnalyzer.Pages.ViewModels.DialogPage;
using System.Windows;

namespace PlcStepAnalyzer.Pages.Views.DialogPage
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowProgressBarView : Window
    {
        public IEventAggregator? EventAggregator { get; set; }

        public ShowProgressBarView()
        {
            InitializeComponent();
            EventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            DataContext = new ShowProgressBarViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 打开背景蒙版
            this.EventAggregator!.GetEvent<DialogEvent>().Publish(true);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.EventAggregator!.GetEvent<DialogEvent>().Publish(false);
        }
    }
}
