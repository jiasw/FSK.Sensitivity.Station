using FSK.Sensitivity.Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SecondarySensitivityViewModel : BaseViewModel
    {
        public SecondarySensitivityViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SecondaryChangeEvent>().Subscribe(OnScreenChange);
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
        


        private string signPath = "";
        public string SignPath
        {
            get { return signPath; }
            set { SetProperty(ref signPath, value); }
        }


        private void OnScreenChange(SecondaryChangeOptions options)
        {
            if (options.Background == SignBackGround.White)
            {
                SelectedColor = new SolidColorBrush(Colors.White);
                TitleColor= new SolidColorBrush(Colors.Black);
            }
            else
            {
                TitleColor = new SolidColorBrush(Colors.White);
                SelectedColor = new SolidColorBrush(Colors.Black);
            }
            SignPath = options.PicUrl;
        }
    }
}
