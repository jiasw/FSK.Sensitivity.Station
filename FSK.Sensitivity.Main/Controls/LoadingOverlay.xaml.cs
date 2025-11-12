using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FSK.Sensitivity.Main.Controls
{
    /// <summary>
    /// LoadingOverlay.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingOverlay : UserControl
    {
        public static readonly DependencyProperty LoadingTextProperty =
           DependencyProperty.Register("LoadingText", typeof(string), typeof(LoadingOverlay),
               new PropertyMetadata("加载中，请稍候..."));
        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }
        public LoadingOverlay()
        {
            InitializeComponent();
        }

        
    }
}
