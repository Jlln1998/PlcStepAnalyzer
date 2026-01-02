using PlcStepAnalyzer.Model;
using PlcStepAnalyzer.Model.DbEntity;
using PlcStepAnalyzer.Model.Enum;
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
            if (analyzerRecord == null)
            {
                return new OpResult();
            }
            switch (analyzerRecord.FileType)
            {
                case PlcFileType.欧姆龙_CSV:
                    // 欧姆龙 CSV 文件解析
                    return await OmronCSVAnalyzer(analyzerRecord);
                case PlcFileType.倍福_CSV:
                    // 倍福 CSV 文件解析
                    return await BeckhoffCSVAnalyzer(analyzerRecord);
                default:
                    return new OpResult() { IsSuccess = false, Message = "不支持的PLC文件类型!" };
            }
        }

        private static async Task<OpResult> OmronCSVAnalyzer(AnalyzerRecord analyzerRecord)
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

                        for (int i = startCol - 1; i < columns.Length; i++)
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
                                        ElapsedTime = indexTime / 1000.0
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

        private static async Task<OpResult> BeckhoffCSVAnalyzer(AnalyzerRecord analyzerRecord)
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

                    // 读取变量行，倍福采用制表符作为分隔符，
                    var VariableLine = reader.ReadLine();
                    string[] columns = VariableLine!.Split('\t');
                    if (columns.Length < startCol)
                    {
                        opResult.Message = $"从第{startRow}行，第{startCol}列位置开始，未查询到变量!";
                        return opResult;
                    }
                    List<string> variableList = new List<string>();
                    for (int i = startCol - 1; i < columns.Length; i += 2) // +2 去除计数列的 SymbolName 列
                    {
                        variableList.Add(columns[i + 1].Trim('\"'));
                    }
                    if (variableList.Count <= 0)
                    {
                        opResult.Message = $"从第{startRow}行，第{startCol}列位置开始，未查询到数据!";
                        return opResult;
                    }

                    // 倍福的变量行和数据行要跳过9行无关数据
                    for (int i = 1; i <= 9; i++)
                    {
                        reader.ReadLine();
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
                    List<string> lineValues = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        columns = line!.Split('\t');
                        if (columns.Length < startCol)
                        {
                            continue;
                        }

                        lineValues.Clear();

                        // 从 startCol 开始截取，并且去除计数列的 Index 序号列
                        // CSV人为算列数的时候 startCol 是从1开始，但是编程数组是从0开始，所以 -1
                        // 因为开始列，每2列为一组，代表是一个变量，组中第一列是Index,第二列才是值，所以 +1
                        // 综上，startCol -1 +1 取到第一个变量列。
                        // 然后每隔 2 列取一次值,所以循环步长为 2
                        for (int i = startCol - 1 + 1; i < columns.Length; i += 2)
                        {
                            lineValues.Add(columns[i]);
                        }
                        // 遍历数据
                        for (int i = 0; i < lineValues.Count && i < varValuesList.Count; i++)
                        {
                            var value = lineValues[i];

                            var oldDate = varValuesList[i].LastOrDefault();
                            if (oldDate != null && oldDate.VarValue == value)
                            {
                                oldDate.ElapsedTime += indexTime / 1000.0;
                            }
                            else
                            {
                                var name = variableList[i];
                                var cf = config.FirstOrDefault(it => it.VarName == name && it.VarValue == value);
                                varValuesList[i].Add(
                                    new AnalyzerRecordItem()
                                    {
                                        RecordId = analyzerRecord.Id,
                                        VarName = name,
                                        VarValue = value,
                                        StationName = cf == null ? name : cf.StationName,
                                        ActionName = cf == null ? value : cf.ActionName,
                                        ElapsedTime = indexTime / 1000.0
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
