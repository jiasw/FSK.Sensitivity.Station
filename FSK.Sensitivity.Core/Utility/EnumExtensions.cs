using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static List<KeyValuePair<string, string>> UserTypes { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("0","管理员"),
            new KeyValuePair<string, string>("1","操作医师")
        };

        public static List<KeyValuePair<string, string>> Genders { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("0","男"),
            new KeyValuePair<string, string>("1","女")
        };
    }
}
