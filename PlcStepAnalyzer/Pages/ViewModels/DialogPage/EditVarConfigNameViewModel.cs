using PlcStepAnalyzer.Model.DbEntity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Pages.ViewModels.DialogPage
{
    public class EditVarConfigNameViewModel(IContainerProvider containerProvider) : BaseDialogViewModel(containerProvider)
    {
        /// <summary>
        /// 要编辑的变量配置表信息
        /// </summary>
        public VarConfigVo? VarConfigVo { get; set; }

        /// <summary>
        /// 输入的新名称
        /// </summary>
        public string NewName
        {
            get { return _newName; }
            set { SetProperty(ref _newName, value); }
        }
        private string _newName = string.Empty;

        /// <summary>
        /// 关闭命令
        /// </summary>
        public DelegateCommand<string> CloseCmd => _closeCmd ??= new DelegateCommand<string>(OnClose);
        private DelegateCommand<string>? _closeCmd;

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            VarConfigVo = parameters.GetValue<VarConfigVo>("VarConfigVo");
        }

        private void OnClose(string type)
        {
            if (type == "Confirm")
            {
                if (string.IsNullOrEmpty(NewName))
                {
                    ErrorMsg = "方案名称不能为空！";
                    return;
                }
                try
                {
                    // 判断该名称是否已经存在
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    var isExist = db.Queryable<VarConfig>().Any(it => it.Name == NewName);
                    if (isExist)
                    {
                        ErrorMsg = "该名称已存在!";
                        return;
                    }

                    // 获取旧数据
                    var config = db.Queryable<VarConfig>().First(it => it.Id == VarConfigVo!.Id && it.Name == VarConfigVo!.ConfigName);

                    // 更新数据
                    config.Name = NewName;
                    db.Updateable<VarConfig>(config).ExecuteCommand();
                }
                catch (Exception ex)
                {
                    ErrorMsg = $"名称修改失败！{ex.Message}";
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
