using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlcStepAnalyzer.Converter
{
    // 行索引转换器：将行索引（从0开始）转为序号（从1开始）
    public class RowIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = DataGridRow，values[1] = DataGrid
            if (values[0] is DataGridRow row && values[1] is DataGrid dataGrid)
            {
                // 获取行的索引（自动适配排序/筛选后的顺序）
                int index = dataGrid.Items.IndexOf(row.DataContext) + 1;
                return index.ToString();
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
