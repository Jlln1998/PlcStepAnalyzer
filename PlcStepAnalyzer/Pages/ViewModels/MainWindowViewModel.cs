using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Events;
using PlcStepAnalyzer.Model;
using PlcStepAnalyzer.Pages.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// 显示的一级菜单列表
        /// </summary>
        public ObservableCollection<SysMenu> FirstMenus { get; } = [];

        /// <summary>
        /// 当前选中的一级菜单名称
        /// </summary>
        private string? _selectFirstMenuName;
        public string? SelectFirstMenuName
        {
            get { return _selectFirstMenuName; }
            set { SetProperty(ref _selectFirstMenuName, value); }
        }

        /// <summary>
        /// 【关于软件】页面菜单
        /// </summary>
        private SysMenu? _aboutAppMenu;
        public SysMenu? AboutAppMenu
        {
            get { return _aboutAppMenu; }
            set { SetProperty(ref _aboutAppMenu, value); }
        }

        /// <summary>
        /// 显示的二级菜单列表，根据当前选中的一级菜单动态变化
        /// </summary>
        public ObservableCollection<SysMenu> SecondMenus { get; } = [];

        /// <summary>
        /// 全局弹窗背景可见性
        /// </summary>
        public Visibility DalogBgVisibility
        {
            get { return _dialogBgVisibility; }
            set { SetProperty(ref _dialogBgVisibility, value); }
        }
        private Visibility _dialogBgVisibility = Visibility.Collapsed;

        /// <summary>
        /// 关于按钮背景可见性
        /// </summary>
        public Visibility AboutBgVisibility
        {
            get { return _aboutBgVisibility; }
            set { SetProperty(ref _aboutBgVisibility, value); }
        }
        private Visibility _aboutBgVisibility = Visibility.Collapsed;

        /// <summary>
        /// 关于页面图标颜色
        /// </summary>
        public Brush AboutIconBrush
        {
            get { return _aboutIconBrush; }
            set { SetProperty(ref _aboutIconBrush, value); }
        }
        private Brush _aboutIconBrush;

        /// <summary>
        /// 一级菜单点击命令
        /// </summary>
        private DelegateCommand<SysMenu>? _menuClickCmd;
        public DelegateCommand<SysMenu> MenuClickCmd => _menuClickCmd ??= new DelegateCommand<SysMenu>(delegate (SysMenu menu)
        {
            OnNavTo(menu, null);
        });

        public MainWindowViewModel(IContainerProvider provider)
        {
            _regionManager = provider.Resolve<IRegionManager>();

            // 初始化一级菜单列表
            this.FirstMenus.AddRange(GlobalData.Instance.SysMenus.Where(it => it.Pid == 0 && !it.Additional));

            // 订阅导航到菜单事件
            provider.Resolve<IEventAggregator>().GetEvent<NavToMenuEvent>().Subscribe(it =>
            {
                var (targetMenu, navParams) = it;
                OnNavTo(targetMenu, navParams);
            });

            this.AboutAppMenu = GlobalData.Instance.SysMenus.FirstOrDefault(it => it.Name == "关于软件");
            this._aboutIconBrush = Application.Current.Resources.FindName("FontColor_Menu") as Brush ?? Brushes.White;

            // 订阅弹窗事件
            provider.Resolve<IEventAggregator>().GetEvent<DialogEvent>().Subscribe(it =>
            {
                DalogBgVisibility = it ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        /// <summary>
        /// 切换到指定菜单
        /// </summary>
        /// <param name="targetMenu">要导航到的菜单</param>
        /// <param name="navParams">传递的导航参数</param>
        public void OnNavTo(SysMenu? targetMenu, NavigationParameters? navParams)
        {
            if (targetMenu == null)
            {
                return;
            }

            if((targetMenu.Id==3 || targetMenu.Id==6) && navParams==null)
            {
                return;
            }

            if (targetMenu.Pid == 0)
            {
                // 修改一级菜单选中状态
                foreach (var firstMenu in FirstMenus)
                {
                    firstMenu.Selected = firstMenu.Id == targetMenu.Id;
                }

                SelectFirstMenuName = targetMenu.Name;

                // 动态加载二级菜单，并且导航到第一个二级菜单
                SecondMenus.Clear();
                SecondMenus.AddRange(GlobalData.Instance.SysMenus.Where(it => it.Pid == targetMenu.Id));
                var showView = SecondMenus.FirstOrDefault();
                if (showView != null)
                {
                    foreach (var secMenu in SecondMenus)
                    {
                        secMenu.Selected = secMenu.Id == showView.Id;
                    }
                    _regionManager.RequestNavigate(MainWindow.REGION_MAIN, showView.ViewName, navParams);
                }
                if (targetMenu.Id == 9)
                {
                    var brush = Application.Current.Resources["BgColor_MenuSelected"] as Brush;
                    AboutIconBrush = brush ?? Brushes.LightCyan;
                    AboutBgVisibility = Visibility.Visible;
                }
                else
                {
                    var brush = Application.Current.Resources["FontColor_Menu"] as Brush;
                    AboutIconBrush = brush ?? Brushes.White;
                    AboutBgVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                // 修改一级菜单选中状态
                foreach (var firstMenu in FirstMenus)
                {
                    firstMenu.Selected = firstMenu.Id == targetMenu.Pid;
                }

                SelectFirstMenuName = GlobalData.Instance.SysMenus.FirstOrDefault(it => it.Id == targetMenu.Pid)?.Name;

                // 动态加载二级菜单，并且导航到指定的二级菜单
                SecondMenus.Clear();
                SecondMenus.AddRange(GlobalData.Instance.SysMenus.Where(it => it.Pid == targetMenu.Pid));
                foreach (var secMenu in SecondMenus)
                {
                    secMenu.Selected = secMenu.Id == targetMenu.Id;
                }
                _regionManager.RequestNavigate(MainWindow.REGION_MAIN, targetMenu.ViewName, navParams);
            }
        }
    }
}
