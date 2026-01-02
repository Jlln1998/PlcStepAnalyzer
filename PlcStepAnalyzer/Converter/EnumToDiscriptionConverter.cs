using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PlcStepAnalyzer.Converter
{
    public class EnumToDiscriptionConverter
    {
        /// <summary>
        /// 枚举值转 DescriptionAttribute 文本的转换器
        /// </summary>
        public class EnumToDescriptionConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // 处理空值（如Nullable枚举为null）
                if (value == null || value == DependencyProperty.UnsetValue)
                    return string.Empty;

                Type enumType = value.GetType();
                if(enumType == null)
                {
                    return value.ToString()??"";
                }

                // 获取枚举字段信息
                Enum enumValue = (Enum)value;
                FieldInfo? fieldInfo = enumType.GetField(enumValue.ToString());
                if (fieldInfo == null)
                    return enumValue.ToString(); 

                // 获取 Description 特性
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                // 有特性返回文本，无特性返回枚举名称
                return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
            }

            /// <summary>
            /// 反向转换（文本 → 枚举值，可选实现）
            /// </summary>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return "";
            }
        }
    }
}
