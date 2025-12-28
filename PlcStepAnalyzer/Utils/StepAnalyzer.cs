using PlcStepAnalyzer.Model;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Pages.ViewModels.DialogPage;
using PlcStepAnalyzer.Pages.Views.DialogPage;
using SqlSugar;
using System.IO;
using System.Windows;

namespace PlcStepAnalyzer.Utils
{
    public class StepAnalyzer
    {
        public static async Task<OpResult> Analyzer(AnalyzerRecord analyzerRecord)
        {
            var opResult = new OpResult();
            var progressBarView = ContainerLocator.Container.Resolve<ShowProgressBarView>();
            progressBarView.Owner = Application.Current.MainWindow;
            try
            {
                // 弹窗提示正在解析数据
                progressBarView.Show();
                var progressVm = (ShowProgressBarViewModel)progressBarView.DataContext;
                string fileName = System.IO.Path.Combine(analyzerRecord.FilePath, analyzerRecord.FileName);
                // 读取 CSV 文件
                using (StreamReader reader = new StreamReader(fileName))
                {
                    Task percent = progressVm.SetProcessStatus(30, "正在定位数据行...");

                    var startRow = analyzerRecord.StartRow;
                    var startCol = analyzerRecord.StartCol;
                    var indexTime = analyzerRecord.IndexTime;

                    // 先去除无关的行
                    for (int i = 1; i < startRow; i++)
                    {
                        reader.ReadLine();
                    }

                    // 读取变量行
                    var VariableLine = reader.ReadLine();
                    string[] columns = VariableLine!.Split(',');
                    List<string> variableList = new List<string>();
                    for (int i = startCol - 1; i < columns.Length; i++)
                    {
                        variableList.Add(columns[i].Trim('\"'));
                    }
                    if (variableList.Count <= 0)
                    {
                        opResult.Message = $"从第{startRow}行，第{startCol}列位置开始，未查询到数据!";
                        return opResult;
                    }

                    // 加载配置项
                    var db = ContainerLocator.Container.Resolve<SqlSugarClient>();
                    var config = db.Queryable<VarConfigItem>().Where(it => it.ConfigId == analyzerRecord.VarConfigId).ToList();

                    // 读取变量值
                    List<List<AnalyzerRecordItem>> varValuesList = new List<List<AnalyzerRecordItem>>();
                    for (int i = 0; i < variableList.Count; i++)
                    {
                        varValuesList.Add(new List<AnalyzerRecordItem>());
                    }

                    await percent;
                    percent = progressVm.SetProcessStatus(60, $"检测到 {variableList.Count} 个变量，正在分析数据...");

                    string? line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        columns = line!.Split(',');

                        for (int i = startCol-1; i < columns.Length; i++)
                        {
                            var value = columns[i].Trim('\"');

                            var oldDate = varValuesList[i - startCol + 1].LastOrDefault();
                            if (oldDate != null && oldDate.VarValue == value)
                            {
                                oldDate.ElapsedTime += indexTime / 1000.0;
                            }
                            else
                            {
                                var name = variableList[i - startCol + 1];
                                var cf = config.FirstOrDefault(it => it.VarName == name && it.VarValue == value);
                                varValuesList[i - startCol + 1].Add(
                                    new AnalyzerRecordItem()
                                    {
                                        RecordId = analyzerRecord.Id,
                                        VarName = name,
                                        VarValue = value,
                                        StationName = cf == null ? name : cf.StationName,
                                        ActionName = cf == null ? value : cf.ActionName,
                                        ElapsedTime = 0.000
                                    }
                                );
                            }
                        }
                    }
                    await percent;
                    percent = progressVm.SetProcessStatus(100, "正在保存分析结果....");

                    foreach (var values in varValuesList)
                    {
                        db.Insertable(values).ExecuteCommand();
                    }
                    await percent;
                    opResult.IsSuccess = true;
                    opResult.Message = "解析成功！";
                    return opResult;
                }
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = $"解析异常：\r\n {ex.Message}";
                return opResult;
            }
            finally
            {
                progressBarView.Close();

            }
        }
    }
}
