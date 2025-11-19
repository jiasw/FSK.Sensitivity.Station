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
        public string name { get; set; }

        public string gender { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string gender_desc
        {
            get
            {

                if (string.IsNullOrWhiteSpace(gender))
                {
                    return "";
                }
                else
                {
                    return EnumExtensions.Genders.Where(g => g.Key == gender).FirstOrDefault().Value;
                }
            }
        }

        public string age { get; set; }

        public string phone { get; set; }

        public string password { get; set; }

        public string type { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string type_desc
        {
            get
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return "";
                }
                else
                {
                    return EnumExtensions.UserTypes.Where(t => t.Key == type).FirstOrDefault().Value;
                }
            }
        }

    }
}
