
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// 云平台解决方案数据项
    /// </summary>
    public class CloudSolutionDataItem
    {
        public string Guid { get; set; }

        public int Type { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not CloudSolutionDataItem other)
                return false;
            return string.Equals(Guid, other.Guid, StringComparison.OrdinalIgnoreCase)
                && Type == other.Type;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Guid?.ToLower(), Type);
        }
        public override string ToString()
        {
            return $"GUID: {Guid}, Type: {Type}";
        }
    }

    public class PrescribeInfo
    {
        public PatientInfoModel Patient { get; set; }
        public string PrescribeId { get; set; }
        public int PrescribeType { get; set; }
        public UserModel Doctor { get; set; }
        public List<ItemOrder> ItemList { get; set; }
    }
    public class ItemOrder
    {
        public string ItemGuid { get; set; }

        public string ItemName { get; set; }
        public int Sort { get; set; }
        public bool State { get; set; }

        /// <summary>
        /// 检查结论
        /// </summary>
        public string Result { get; set; }



        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 检查参数模型
        /// </summary>
        
        public JsonElement ItemParam { get; set; }


        /// <summary>
        /// 检查结果数据
        /// </summary>
        public string ItemData { get; set; }


    }
    

   
    public abstract class ItemParamBase
    {
        /// <summary>
        /// 瞳距
        /// </summary>
        public int Pupillary { get; set; } = 50;
    }
    public class CheckTimesParam : ItemParamBase
    {
        public int CheckTimes { get; set; }
    }
    public class EyeTestParam : ItemParamBase
    {
       
        public int EyeType { get; set; }
        public int DazzleLight { get; set; }
        public int TimeSlot { get; set; }

        public int Distance { get; set; }
    }

    public class UserModel
    {
        
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 角色标识
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorId { get; set; }
        /// <summary>
        /// 医师名
        /// </summary>
        public string DoctorName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string DoctorTimestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = false;
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; } = 1;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 医生级别
        /// </summary>
        public string ModelName { get; set; }

        public bool AsyncState { get; set; }
    }

    /// <summary>
    /// 患者信息
    /// </summary>
    public class PatientInfoModel
    {
        
        public int? Id { get; set; }
        public string StudentGuid { get; set; }
        public string StudentName { get; set; }

        /// <summary>
        /// 两者关系（1父子/女2母子/女3其他）
        /// </summary>
        public int Relation { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public string PatientIdNumber { get; set; }
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
        public int Age { get; set; }
        /// <summary>
        ///患者身份证号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public string PlatformGuid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string Clas { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string GradeClass => Grade + " " + Clas;
        /// <summary>
        /// 瞳距
        /// </summary>
        public int Pupillary { get; set; } = 64;
        /// <summary>
        /// 左眼度数
        /// </summary>
        public int DegreesLeft { get; set; }
        /// <summary>
        /// 右眼度数
        /// </summary>
        public int DegreesRight { get; set; }
        /// <summary>
        /// 左眼散光
        /// </summary>
        public int AstigmiaLeft { get; set; }
        /// <summary>
        /// 右眼散光
        /// </summary>
        public int AstigmiaRight { get; set; }
        /// <summary>
        /// 左眼轴位
        /// </summary>
        public int ALeft { get; set; } //A
        /// <summary>
        /// 右眼轴位
        /// </summary>
        public int ARight { get; set; } //右轴度A
        //
        public string DiopterLeft { get; set; }//屈光度左眼
        public string DiopterRight { get; set; }//屈光度右眼
        public string CVALeft { get; set; }//矫正视力左眼
        public string CVARight { get; set; }//矫正视力右眼
        public string DownLight { get; set; }//下夹光ADD
        public string HDPrism { get; set; }//HD棱镜
        public string VDPrism { get; set; }//VD棱镜

        public string ConclusionOS { get; set; }//结论OS
        public string ConclusionOD { get; set; }//结论OD

        /// <summary>
        /// 是否同步
        /// </summary>
        public bool AsyncState { get; set; }
        public bool Checked { get; set; }
    }


    public class ReportResult<T>
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
