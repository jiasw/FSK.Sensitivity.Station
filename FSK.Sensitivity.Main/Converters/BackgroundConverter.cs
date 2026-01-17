using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FSK.Sensitivity.Main.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null || string.IsNullOrWhiteSpace( value.ToString() ))
            {
                return Brushes.White;
            }
            else
            {
                
                    
                    
                // 2. 将字符串转换为 BitmapImage
                BitmapImage image = new BitmapImage();
                image.BeginInit();

                // 处理绝对路径和相对路径（如果是网络图片，UriKind 选 Absolute）
                image.UriSource = new Uri("pack://application:,,,/FSK.Sensitivity.Main;component/Resource/Images/Sign/" + value.ToString(), UriKind.RelativeOrAbsolute);

                // 性能优化：在加载时缓存，避免文件被占用
                image.CacheOption = BitmapCacheOption.OnLoad;

                image.EndInit();
                return image;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
