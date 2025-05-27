using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FSK.Sensitivity.Main.Controls
{
    public class CustomScrollViewer : ScrollViewer
    {
        private Point _lastTouchPoint;

        protected override void OnTouchDown(TouchEventArgs e)
        {
            _lastTouchPoint = e.GetTouchPoint(this).Position;
            CaptureTouch(e.TouchDevice);
            e.Handled = true;
            base.OnTouchDown(e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            Point currentTouchPoint = e.GetTouchPoint(this).Position;
            Vector delta = currentTouchPoint - _lastTouchPoint;

            if (delta.Y != 0)
            {
                ScrollToVerticalOffset(VerticalOffset - delta.Y);
            }

            _lastTouchPoint = currentTouchPoint;
            e.Handled = true;
            base.OnTouchMove(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;
            base.OnTouchUp(e);
        }
    }
}
