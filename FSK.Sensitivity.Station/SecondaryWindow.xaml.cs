using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
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
using WpfScreenHelper;
using Prism.Ioc;

namespace FSK.Sensitivity.Station
{
    /// <summary>
    /// SecondaryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        private readonly IRegionManager regionManager;

        public SecondaryWindow(IRegionManager regionManager, IContainerExtension container)
        {
            InitializeComponent();

           

            this.regionManager =  container.Resolve<IRegionManager>(); ;
            //设置区域后重新刷新
            RegionManager.SetRegionManager(this, regionManager);
            RegionManager.UpdateRegions();
            Screen s = Screen.AllScreens.Where(s => !s.Primary).First();

            if (s != null)
            {
                this.Top = s.WorkingArea.Top;
                this.Left = s.WorkingArea.Left;
            }
            else
            {
                this.Top = Screen.PrimaryScreen.WorkingArea.Top;
                this.Left = Screen.PrimaryScreen.WorkingArea.Left;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState=WindowState.Maximized;
            this.WindowStyle=WindowStyle.None;
            regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Init);
        }
    }
}
