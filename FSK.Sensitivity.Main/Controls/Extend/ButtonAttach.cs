using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace FSK.Sensitivity.Main.Controls.Extend
{
    public class ButtonAttach
    {
        public static ImageSource GetImageSource(DependencyObject obj) =>
        (ImageSource)obj.GetValue(ImageSourceProperty);
        public static void SetImageSource(DependencyObject obj, ImageSource value) =>
            obj.SetValue(ImageSourceProperty, value);
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.RegisterAttached("ImageSource",
                typeof(ImageSource),
                typeof(ButtonAttach),
                new FrameworkPropertyMetadata(null));
    }
}
