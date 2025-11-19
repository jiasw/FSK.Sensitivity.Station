using FSK.Sensitivity.Core.Const;
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

namespace FSK.Sensitivity.Main.Views
{
    /// <summary>
    /// MainMenu.xaml 的交互逻辑
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private readonly IRegionManager regionManager;

        public MainMenu(IRegionManager regionManager)
        {
            InitializeComponent();
            this.regionManager = regionManager;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_SensitivitySetting);
        }

        private void MainMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
