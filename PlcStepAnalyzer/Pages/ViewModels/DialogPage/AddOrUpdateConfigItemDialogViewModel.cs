using PlcStepAnalyzer.Model.DbEntity;
using SqlSugar;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class AddOrUpdateConfigItemDialogViewModel(IContainerProvider containerProvider) : BaseDialogViewModel(containerProvider)
    {
        /// <summary>
        /// 是否是编辑模式
        /// </summary>
        public bool IsEdit
        {
            get { return _isEdit; }
            set { SetProperty(ref _isEdit, value); }
        }
        private bool _isEdit;

        /// <summary>
        /// 输入的配置项信息
        /// </summary>
        public VarConfigItem InputVarConfigItem
        {
            get { return _inputVarConfigItem; }
            set { SetProperty(ref _inputVarConfigItem, value); }
        }
        private VarConfigItem _inputVarConfigItem = new VarConfigItem();

        /// <summary>
        /// 关闭事件
        /// </summary>
        public DelegateCommand<string> CloseCmd => _closeCmd ??= new DelegateCommand<string>(OnClose);
        private DelegateCommand<string>? _closeCmd;


        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            var oldOldVarConfigItem = parameters.GetValue<VarConfigItem>("OldVarConfigItem");
            if (oldOldVarConfigItem != null)
            {
                InputVarConfigItem = oldOldVarConfigItem;
                if (oldOldVarConfigItem.Id != Guid.Empty)
                {
                    IsEdit = true;
                    Title = "编辑变量配置项";
                }
                else
                {
                    Title = "新增变量配置项";
                }
            }
        }

        private void OnClose(string type)
        {
            if (type == "Confirm")
            {
                if (string.IsNullOrEmpty(InputVarConfigItem.VarName))
                {
                    ErrorMsg = "请输入变量名!";
                    return;
                }
                if (string.IsNullOrEmpty(InputVarConfigItem.VarValue))
                {
                    ErrorMsg = "请输入变量值!";
                    return;
                }
                if (string.IsNullOrEmpty(InputVarConfigItem.StationName))
                {
                    ErrorMsg = "请输入工位名!";
                    return;
                }
                if (string.IsNullOrEmpty(InputVarConfigItem.ActionName))
                {
                    ErrorMsg = "请输入步骤名!";
                    return;
                }

                if (InputVarConfigItem.Id == Guid.Empty)
                {
                    InputVarConfigItem.Id = Guid.NewGuid();
                }

                var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                var isExist = db.Queryable<VarConfigItem>()
                    .Any(it => it.VarName == InputVarConfigItem.VarName && 
                        it.VarValue == InputVarConfigItem.VarValue && 
                        it.ConfigId == InputVarConfigItem.ConfigId &&
                        it.Id != InputVarConfigItem.Id);
                if (isExist)
                {
                    ErrorMsg = $"已存在变量:[{InputVarConfigItem.VarName}],值:[{InputVarConfigItem.VarValue}]的配置！";
                    return;
                }
                var count = db.Storageable(InputVarConfigItem).ExecuteCommand();
                if(count>0)
                {
                    db.Updateable<VarConfig>()
                           .SetColumns(it => new VarConfig()
                           {
                               UpdateTime = DateTime.Now,
                           })
                           .Where(it => it.Id == InputVarConfigItem.ConfigId)
                           .ExecuteCommand();
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
