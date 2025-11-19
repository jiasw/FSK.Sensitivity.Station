using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace FSK.Sensitivity.Main.Converters
{
    public class LightStatusToBoolConverter : IValueConverter
    {
        // 将枚举转换成是否选中
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LightStatus status)
            {
                return status == LightStatus.Strong;
            }
            return DependencyProperty.UnsetValue;
        }

        // 将布尔值转回枚举
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                return isChecked ? LightStatus.Strong : LightStatus.Off;
            }
            return DependencyProperty.UnsetValue;
        }
    }

}
