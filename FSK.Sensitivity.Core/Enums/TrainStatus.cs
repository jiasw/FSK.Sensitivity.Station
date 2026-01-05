using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum TrainStatus
    {
        /// <summary>
        /// 待训练
        /// </summary>
        [Description("待训练")]
        Pending,
        /// <summary>
        /// 训练中
        /// </summary>
        [Description("训练中")]
        Training,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Trained,
        /// <summary>
        /// 不需训练
        /// </summary>
        [Description("不需训练")]
        NotTrain

    }
}
