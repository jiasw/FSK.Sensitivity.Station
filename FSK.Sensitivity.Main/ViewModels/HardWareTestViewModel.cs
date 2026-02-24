using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class HardWareTestViewModel : BaseViewModel, IDialogAware
    {
        private string title = "硬件测试";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

       
        private readonly ILight light;
        private readonly IMotor motor;
        private readonly IJoystick joystick;

        public HardWareTestViewModel(ILight light,IMotor motor, IJoystick joystick)
        {
            this.light = light;
            this.motor = motor;
            this.joystick = joystick;
        }

        public DelegateCommand<string> LightCommand => new DelegateCommand<string>(Light);
        private void Light(string str)
        {
            if (str == "0")
            {
                light.TurnOffAll();
            }
            else
            {
                light.TurnOnAll();
            }
        }

        public DelegateCommand InitMotorCommand => new DelegateCommand(InitMotor);
        private void InitMotor()
        {
            motor.Initialize();
        }

        private short slideBlockPosition;
        public short SlideBlockPosition
        {
            get { return slideBlockPosition; } 
            set { SetProperty(ref slideBlockPosition, value); MoveMotor(); }

        }

        public DelegateCommand MoveMotorCommand => new DelegateCommand(MoveMotor);
        private void MoveMotor()
        {
            //motor.SetSlideBlock(SlideBlockPosition);
        }

        private short leftClampPosition;
        public short LeftClampPosition
        {
            get { return leftClampPosition; }
            set { SetProperty(ref leftClampPosition, value); MoveLeftClamp(); }
        }
        public DelegateCommand MoveLeftClampCommand => new DelegateCommand(MoveLeftClamp);
        private void MoveLeftClamp()
        {
            motor.SetLeftDisk(LeftClampPosition);
        }
        private short rightClampPosition;
        public short RightClampPosition
        {
            get { return rightClampPosition; }
            set { SetProperty(ref rightClampPosition, value); MoveRightClamp(); }
        }
        public DelegateCommand MoveRightClampCommand => new DelegateCommand(MoveRightClamp);
        private void MoveRightClamp()
        {
            motor.SetRightDisk(RightClampPosition);
        }

        private string btnJoystickContent="启动摇杆监听";
        public string BtnJoystickContentProperty
        {
            get { return btnJoystickContent; }
            set { SetProperty(ref btnJoystickContent, value); }
        }


        private bool isJoystickEnabled=false;
        public DelegateCommand StartOrStopJoystickCommand => new DelegateCommand(StartOrStopJoystick);

        private void StartOrStopJoystick()
        {
            if (isJoystickEnabled) {
                joystick.StopMonitoring();
                joystick.Pressed -= Joystick_Pressed;
                isJoystickEnabled = true;
                BtnJoystickContentProperty = "启动摇杆监听";
                JoystickStatus = "";
            }
            else
            {
                joystick.StartMonitoring();
                joystick.Pressed += Joystick_Pressed;
                isJoystickEnabled = false;
                BtnJoystickContentProperty = "停止摇杆监听";
            }
        }

        private string joystickStatus="摇杆监听...";
        public string JoystickStatus
        {
            get { return joystickStatus; }
            set { SetProperty(ref joystickStatus, value); }
        }


        private void Joystick_Pressed(object? sender, JoystickEventArgs e)
        {
            JoystickStatus+= $"[{e.Timestamp:HH:mm:ss}] 按下 {e.Command}\n";
        }
        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
