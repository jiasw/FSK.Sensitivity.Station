using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FSK.Sensitivity.Main.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            if (value is DCKTime timeEnum)
            {
                var field = value.GetType().GetField(value.ToString());
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                return attribute != null ? attribute.Description : value.ToString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要从界面反向写回枚举，可以在这里处理，但通常用于 Display 不需要
            return Binding.DoNothing;
        }
    }
}
