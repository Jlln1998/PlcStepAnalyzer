using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Pages.ViewModels.DialogPage;
using PlcStepAnalyzer.Pages.Views;
using PlcStepAnalyzer.Pages.Views.DialogPage;
using PlcStepAnalyzer.Utils;
using Serilog;
using SqlSugar;
using System.Diagnostics;
using System.Windows;

namespace PlcStepAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var mainWindow = Container.Resolve<MainWindow>();

            return mainWindow;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            if(!CheckSingle())
            {
                Current.Shutdown();
                return;
            }

            var oldConfig = ConfigFileHelper.LoadConfig();
            if (oldConfig == null)
            {
                GlobalData.Instance.DataConfig = new DataConfig();
                ConfigFileHelper.SaveConfig(GlobalData.Instance.DataConfig);
            }
            else
            {
                GlobalData.Instance.DataConfig = oldConfig;
            }
            base.OnStartup(e);
        }

        protected override void InitializeShell(Window shell)
        {
            var db = Container.Resolve<SqlSugarClient>();
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(
                [
                    typeof(VarConfig),
                    typeof(VarConfigItem),
                    typeof(AnalyzerRecord),
                    typeof(AnalyzerRecordItem),
                ]
            );

            base.InitializeShell(shell);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("AppDatas/Logs/log-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            containerRegistry.Register<SqlSugarClient>(() =>
            {
                var db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "datasource=AppDatas/Data.db",
                    DbType = SqlSugar.DbType.Sqlite,
                    IsAutoCloseConnection = true
                },
                db =>
                {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
#if DEBUG
                        Debug.WriteLine(UtilMethods.GetSqlString(SqlSugar.DbType.Sqlite, sql, pars));
#endif
                    };
                });
                return db;
            });

            containerRegistry.Register<MainWindow>();
            containerRegistry.RegisterForNavigation<AnalyzerRecordView>();
            containerRegistry.RegisterForNavigation<AnalyzerRecordItemView>();
            containerRegistry.RegisterForNavigation<VarConfigListView>();
            containerRegistry.RegisterForNavigation<VarConfigItemListView>();
            containerRegistry.RegisterForNavigation<GlobalConfigView>();
            containerRegistry.RegisterForNavigation<AboutView>();

            containerRegistry.RegisterDialog<NoticeDialogView, NoticeDialogViewModel>();
            containerRegistry.RegisterDialog<AddVarConfigDialogView, AddVarConfigDialogViewModel>();
            containerRegistry.RegisterDialog<EditVarConfigNameView, EditVarConfigNameViewModel>();
            containerRegistry.RegisterDialog<AddOrUpdateConfigItemDialogView, AddOrUpdateConfigItemDialogViewModel>();
            containerRegistry.RegisterDialog<NewAnalyzerRecordView, NewAnalyzerRecordViewModel>();
        }
    }
}
