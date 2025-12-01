using FSK.Sensitivity.Core.EventBus;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SecondaryContrastViewModel : BaseViewModel
    {
        public SecondaryContrastViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<ContrastSignChangeEvent>().Subscribe(OnScreenChange);
        }
        private SolidColorBrush selectedColor = new SolidColorBrush(Colors.White);
        public SolidColorBrush SelectedColor
        {
            get { return selectedColor; }
            set { SetProperty(ref selectedColor, value); }
        }

        private SolidColorBrush titileColor = new SolidColorBrush(Colors.White);
        public SolidColorBrush TitleColor
        {
            get { return titileColor; }
            set { SetProperty(ref titileColor, value); }
        }
        private SolidColorBrush backColor = new SolidColorBrush(Colors.White);
        public SolidColorBrush BackColor
        {
            get { return backColor; }
            set { SetProperty(ref backColor, value); }
        }


        private string signPath = "";
        public string SignPath
        {
            get { return signPath; }
            set { SetProperty(ref signPath, value); }
        }

        private int selectedIndex = 1;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }


        private void OnScreenChange(ContrastChangeSignOptions options)
        {
            SignImage = null;
            if (options.Brush == SignBackGround.White)
            {
                BackColor = new SolidColorBrush(Colors.White);
                TitleColor = new SolidColorBrush(Colors.Black);
            }
            else
            {
                BackColor = new SolidColorBrush(Colors.Black);
                TitleColor = new SolidColorBrush(Colors.White);
            }
            if (!string.IsNullOrWhiteSpace(options.PicturePath))
            {
                SignPath = "pack://application:,,,/FSK.Sensitivity.Main;component/Resource/Images/Sign/" + options.PicturePath;
                BitmapImage bitmapImage = new BitmapImage(new Uri(SignPath));
                bitmapImage.Freeze();
                SignImage = bitmapImage;
            }


        }

        private BitmapImage _signImage;
        private readonly IEventAggregator eventAggregator;

        public BitmapImage SignImage
        {
            get { return _signImage; }
            set { SetProperty(ref _signImage, value); }
        }

        public DelegateCommand SelectCommand => new DelegateCommand(Select);

        private void Select()
        {
            eventAggregator.GetEvent<ContrastSelectedEvent>().Publish(SelectedIndex);
            selectedIndex = -1;
            Growl.Info("选择成功");
        }
    }
}
