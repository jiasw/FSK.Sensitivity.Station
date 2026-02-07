using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class Manger:RootEntity
    {
        public string Name { get; set; }

        public string Gender { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string gender_desc
        {
            get
            {

                if (string.IsNullOrWhiteSpace(Gender))
                {
                    return "";
                }
                else
                {
                    int typecode = int.Parse(Gender);
                    var enumModel = EnumExtensions.ToEnumModelList<Gender>().FirstOrDefault(t => t.Value == typecode);
                    return enumModel?.Description ?? "";
                }
            }
        }

        public string Age { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; } = "112233";

        public string Type { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string type_desc
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Type))
                {
                    return "";
                }
                else
                {
                    int typecode = int.Parse(Type);
                    var enumModel = EnumExtensions.ToEnumModelList<UserType>().FirstOrDefault(t => t.Value == typecode);
                    return enumModel?.Description ?? "";
                }
            }
        }

        public string Email { get; set; }

        public string DoctorID { get; set; }

        public string DoctorName { get; set; }

    }
}
