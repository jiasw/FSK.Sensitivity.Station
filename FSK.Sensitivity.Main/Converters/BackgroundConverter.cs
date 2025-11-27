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
            if(value is null)
            {
                return Brushes.White;
            }
            if(value.ToString().Equals("black", StringComparison.CurrentCultureIgnoreCase) || value.ToString().Equals("white", StringComparison.CurrentCultureIgnoreCase))
            {
                return value.ToString().ToLower() == "black"? Brushes.Black : Brushes.White;
            }
            else
            {
                var image = new BitmapImage(new Uri("pack://application:,,,/FSK.Sensitivity.Main;component/Resource/Images/Sign/"+value.ToString()));
                return new ImageBrush(image)
                {
                    Stretch = Stretch.Fill
                };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
