using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum TrainStatus
    {
        [System.ComponentModel.Description("待训练")]
        Pending,
        [System.ComponentModel.Description("训练中")]
        Training,
        [System.ComponentModel.Description("已完成")]
        Trained,
        [System.ComponentModel.Description("未训练")]
        NotTrain

    }
}
