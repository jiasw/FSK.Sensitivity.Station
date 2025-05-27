using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface IMotor
    {
        bool IsAvailable { get; }
        void Move(int speed, int direction);
    }
}
