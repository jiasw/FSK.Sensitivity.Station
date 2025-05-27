using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface ILight
    {

        bool IsAvailable { get; }
        void TurnOn();
        void TurnOff();
    }
}
