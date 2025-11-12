using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    public class Patient: RootEntity
    {
       
        //public string StudentGuid { get; set; }
        //public string StudentName { get; set; }

        ///// <summary>
        ///// 两者关系（1父子/女2母子/女3其他）
        ///// </summary>
        //public int Relation { get; set; }
        ///// <summary>
        ///// 联系人姓名
        ///// </summary>
        //public string ContactName { get; set; }
        ///// <summary>
        ///// 患者编号
        ///// </summary>
        //public string PatientIdNumber { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 患者性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int Age { get; set; }
        /// <summary>
        ///患者身份证号
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string IdCard { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Address { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string PlatformGuid { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string School { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Grade { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Class { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string GradeClass => Grade + " " + Class;
        ///// <summary>
        ///// 瞳距
        ///// </summary>
        //public int Pupillary { get; set; } = 64;
        ///// <summary>
        ///// 左眼度数
        ///// </summary>
        //public int DegreesLeft { get; set; }
        ///// <summary>
        ///// 右眼度数
        ///// </summary>
        //public int DegreesRight { get; set; }
        ///// <summary>
        ///// 左眼散光
        ///// </summary>
        //public int AstigmiaLeft { get; set; }
        ///// <summary>
        ///// 右眼散光
        ///// </summary>
        //public int AstigmiaRight { get; set; }
        ///// <summary>
        ///// 左眼轴位
        ///// </summary>
        //public int ALeft { get; set; } //A
        ///// <summary>
        ///// 右眼轴位
        ///// </summary>
        //public int ARight { get; set; } //右轴度A
        ////
        //public string DiopterLeft { get; set; }//屈光度左眼
        //public string DiopterRight { get; set; }//屈光度右眼
        //public string CVALeft { get; set; }//矫正视力左眼
        //public string CVARight { get; set; }//矫正视力右眼
        //public string DownLight { get; set; }//下夹光ADD
        //public string HDPrism { get; set; }//HD棱镜
        //public string VDPrism { get; set; }//VD棱镜

        //public string ConclusionOS { get; set; }//结论OS
        //public string ConclusionOD { get; set; }//结论OD

        /// <summary>
        /// 是否同步
        /// </summary>
        public bool AsyncState { get; set; }
        public bool Checked { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Gender => Sex == 1 ? "男" : "女";

        
    }
}
