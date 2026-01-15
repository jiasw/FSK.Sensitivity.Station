using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum DayOrNight
    {
        [Description("日")]
        Day = 1,
        [Description("夜")]
        Night
    }

    public enum SignBackGround
    {
        //白色背景
        White = 1,
        //黑色背景
        Black,
    }

    /// <summary>
    /// 环境模拟CSF检查
    /// </summary>
    public enum CSFVA
    {
        [Description("空")]
        None=0,
        [Description("0.06")]
        VA06,
        [Description("0.1")]
        VA10,
        [Description("0.2")]
        VA20,
        [Description("0.4")]
        VA40,
        [Description("0.6")]
        VA60,
        [Description("0.8")]
        VA80
    }

    public enum CSFValue
    {
        [Description("无")]
        None=0,
        [Description("1")]
        CS001,
        [Description("6")]
        CS006,
        [Description("9")]
        CS009,
        [Description("12")]
        CS012,
        [Description("20")]
        CS020,
        [Description("30")]
        CS030,
        [Description("45")]
        CS045,
        [Description("66")]
        CS066,
        [Description("100")]
        CS100,
    }

    public enum DCKTime
    {
        [Description("5")]
        T05 = 5,
        [Description("10")]
        T10=10,
        [Description("15")]
        T15=15,
        [Description("20")]
        T20=20,
        [Description("30")]
        T30=30,
        [Description("50")]
        T50=50
    }

    public enum DCKValue
    {
        [Description("0")]
        None,
        [Description("0.1")]
        VA010,
        [Description("0.15")]
        VA015,
        [Description("0.2")]
        VA020,
        [Description("0.25")]
        VA025,
        [Description("0.3")]
        VA030,
        [Description("0.4")]
        VA040,
        [Description("0.5")]
        VA050,
        [Description("0.6")]
        VA060,
        [Description("0.8")]
        VA080,
        [Description("1.0")]
        VA100
    }
}
