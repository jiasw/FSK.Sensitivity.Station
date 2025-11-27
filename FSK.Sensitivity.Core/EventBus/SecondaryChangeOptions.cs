using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        //暗适应检测
        Contrast,
    }

    public enum SignBackGround
    {
        //白色背景
        White = 1,
        //黑色背景
        Black,
    }
    /// <summary>
    /// 副屏切换内容事件
    /// </summary>
    public class SecondaryChangeOptions
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public ChangeAction Action { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public SignBackGround Brush { get; set; }
    }

    /// <summary>
    /// 敏感度视标切换信号
    /// </summary>
    public class SensitivityChangeSignOptions
    {
        /// <summary>
        /// 视标图片路径
        /// </summary>
        public string PicturePath { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public SignBackGround BackgroundBrush { get; set; }
    }

    /// <summary>
    /// 适应度检测信号
    /// </summary>
    public class ContrastChangeSignOptions
    {
        /// <summary>
        /// 适应度图片路径
        /// </summary>
        public string PicturePath { get; set; }

        /// <summary>
        /// 适应度背景色
        /// </summary>
        public SignBackGround Brush { get; set; }
    }
}
