using DryIoc;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SecondarySensitivityViewModel : BaseViewModel, INavigationAware
    {

        JoystickEvent joystickEvent;
        public SecondarySensitivityViewModel(IEventAggregator eventAggregator, IJoystick joystick)
        {
            this.eventAggregator = eventAggregator;
            this.joystick = joystick;
            eventAggregator.GetEvent<SensitivitySignChangeEvent>().Subscribe(OnScreenChange);
            joystickEvent= eventAggregator.GetEvent<JoystickEvent>();
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


        private void OnScreenChange(SensitivityChangeSignOptions options)
        {
            
            if (options.BackgroundBrush == SignBackGround.White)
            {
                BackColor = new SolidColorBrush(Colors.White);
                TitleColor= new SolidColorBrush(Colors.Black);
            }
            else
            {
                BackColor = new SolidColorBrush(Color.FromRgb(153, 153, 153));
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedIndex = 1;
            BackColor= new SolidColorBrush(Colors.White);
            joystick.StartMonitoring();
            joystick.Pressed += Joystick_Pressed;
        }

        private void Joystick_Pressed(object? sender, Core.HardWare.Drivers.JoystickEventArgs e)
        {
            ActionArgs args = null;
            if (e.Command == JoystickStatus.Confirm)
            {
                args = new ActionArgs()
                {
                    Index = selectedIndex,
                    Action = new Core.HardWare.Drivers.JoystickEventArgs(JoystickStatus.Confirm)
                };

            }
            else
            {
                //if (e.Command == JoystickStatus.Front)
                //{
                //    if (SelectedIndex > 5)
                //    {
                //        SelectedIndex -= 5;
                //    }
                //}
                //else if (e.Command == JoystickStatus.Back)
                //{
                //    if (SelectedIndex < 5)
                //    {
                //        SelectedIndex += 5;
                //    }
                //}
                //else 
                
                
                if (e.Command == JoystickStatus.Left)
                {
                    if (SelectedIndex > 1)
                    {
                        SelectedIndex -= 1;
                    }
                }
                else if (e.Command == JoystickStatus.Right)
                {
                    if (SelectedIndex < 10)
                    {
                        SelectedIndex += 1;
                    }
                    
                }
                args = new ActionArgs()
                {
                    Index = -1,
                    Action = new Core.HardWare.Drivers.JoystickEventArgs(e.Command)
                };
            }
            joystickEvent.Publish(args);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            joystick.Pressed -= Joystick_Pressed;
            joystick.StopMonitoring();
            
        }

        private BitmapImage _signImage;
        private readonly IEventAggregator eventAggregator;
        private readonly IJoystick joystick;
        private readonly IMotor motor;

        public BitmapImage SignImage
        {
            get { return _signImage; }
            set { SetProperty(ref _signImage, value); }
        }


        public DelegateCommand SelectCommand => new DelegateCommand(Select);

        private void Select()
        {
            ActionArgs args = new ActionArgs()
            {
                Index = selectedIndex,
                Action = new Core.HardWare.Drivers.JoystickEventArgs(JoystickStatus.Confirm)
            };
            joystickEvent.Publish(args);
            Growl.Info("选择成功");
        }

    }
}
