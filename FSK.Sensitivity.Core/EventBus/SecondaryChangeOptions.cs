using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    public enum ChangeAction
    {
        //待机
        Idle = 1,
        //对比敏感度
        Sensitivity,
        //按适应检测
        Contrast,
    }

    public enum SignBackGround
    {
        //白色背景
        White = 1,
        //黑色背景
        Black,
    }

    public class SecondaryChangeOptions
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public ChangeAction Action { get; set; }

        /// <summary>
        /// 图片背景色
        /// </summary>
        public SignBackGround Background { get; set; }=SignBackGround.White;

        /// <summary>
        /// 图片路径
        /// </summary>
        public string PicUrl { get; set; }
    }
}
