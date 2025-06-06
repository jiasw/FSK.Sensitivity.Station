using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.Controls.Extend
{
    public class ButtonActiveAttach
    {
        public static readonly DependencyProperty IsButtonActiveProperty =
        DependencyProperty.RegisterAttached("IsButtonActive",
            typeof(bool),
            typeof(ButtonActiveAttach),
            new PropertyMetadata(false));

        public static void SetIsButtonActive(DependencyObject obj, bool value)
            => obj.SetValue(IsButtonActiveProperty, value);

        public static bool GetIsButtonActive(DependencyObject obj)
            => (bool)obj.GetValue(IsButtonActiveProperty);
    }
}
