using PlcStepAnalyzer.Model.DbEntity;
using SqlSugar;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class AddVarConfigDialogViewModel(IContainerProvider containerProvider) : BaseDialogViewModel(containerProvider)
    {
        /// <summary>
        /// 新配置名
        /// </summary>
        public string NewConfigName
        {
            get { return _newConfigName; }
            set { SetProperty(ref _newConfigName, value); }
        }
        private string _newConfigName = string.Empty;

        /// <summary>
        /// 关闭命令
        /// </summary>
        public DelegateCommand<string> CloseCmd => _closeCmd ??= new DelegateCommand<string>(OnClose);
        private DelegateCommand<string>? _closeCmd;


        private void OnClose(string type)
        {
            if (type == "Confirm")
            {
                if (string.IsNullOrEmpty(NewConfigName))
                {
                    ErrorMsg = "配置表名称不能为空！";
                    return;
                }

                // 判断该名称是否已经存在
                var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                var isExist = db.Queryable<VarConfig>().Any(it => it.Name == NewConfigName);
                if (isExist)
                {
                    ErrorMsg = "该名称已存在!";
                    return;
                }

                // 新增配置表数据
                try
                {
                    db.Insertable<VarConfig>(new VarConfig() { Id = new Guid(), Name = NewConfigName }).ExecuteCommand();
                }
                catch (Exception ex)
                {
                    ErrorMsg= $"新增失败！{ex.Message}";
                    return;
                }
                RequestClose.Invoke(ButtonResult.OK);
            }
            else
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
        }
    }
}
