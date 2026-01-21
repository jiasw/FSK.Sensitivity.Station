using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.Controls.Extend
{
    public class ButtonClickAttach
    {
        public static readonly DependencyProperty IsButtonClickProperty =
            DependencyProperty.RegisterAttached("IsButtonClick",
            typeof(bool),
            typeof(ButtonClickAttach),
            new PropertyMetadata(false));
        public static bool GetIsButtonClick(DependencyObject obj)=>
            (bool)obj.GetValue(IsButtonClickProperty);
        public static void SetIsButtonClick(DependencyObject obj, bool value)
            => obj.SetValue(IsButtonClickProperty, value);
    }
}
