using FSK.Sensitivity.Core.Const;
using Prism.Ioc;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FSK.Sensitivity.Station
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainerProvider containerProvider;

        public MainWindow(IContainerProvider containerProvider)
        {
            InitializeComponent();
            this.containerProvider = containerProvider;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var secondaryWindow = containerProvider.Resolve<SecondaryWindow>();
            secondaryWindow.Show();
        }
    }
}