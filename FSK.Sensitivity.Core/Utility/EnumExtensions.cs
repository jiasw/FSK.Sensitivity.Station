using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace FSK.Sensitivity.Core
{

    public class EnumModel
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public static class EnumExtensions
    {
       
       
        public static List<KeyValuePair<string, string>> Genders { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("0","男"),
            new KeyValuePair<string, string>("1","女")
        };

        /// <summary>
        /// 将枚举类型转换为EnumModel列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>EnumModel列表</returns>
        public static List<EnumModel> ToEnumModelList<T>() where T : Enum
        {
            var type = typeof(T);
            var values = Enum.GetValues(type).Cast<T>();

            return values.Select(e => new EnumModel
            {
                Value = Convert.ToInt32(e),
                Name = e.ToString(),
                Description = GetEnumDescription(e)
            }).ToList();
        }
        /// <summary>
        /// 将单个枚举值转换为EnumModel
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>EnumModel实例</returns>
        public static EnumModel ToEnumModel(this Enum enumValue)
        {
            if (enumValue == null)
                return new EnumModel();
            return new EnumModel
            {
                Value = Convert.ToInt32(enumValue),
                Name = enumValue.ToString(),
                Description = GetEnumDescription(enumValue)
            };
        }
        /// <summary>
        /// 获取指定的枚举值对应的EnumModel
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举数值</param>
        /// <returns>EnumModel实例</returns>
        public static EnumModel GetEnumModel<T>(this int value) where T : Enum
        {
            var enumValue = (T)Enum.ToObject(typeof(T), value);
            return enumValue.ToEnumModel();
        }
        /// <summary>
        /// 获取指定的枚举名称对应的EnumModel
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="name">枚举名称</param>
        /// <returns>EnumModel实例</returns>
        public static EnumModel GetEnumModel<T>(this string name) where T : Enum
        {
            if (Enum.TryParse(typeof(T), name, out var result) && result != null)
            {
                return ((T)result).ToEnumModel();
            }

            return new EnumModel();
        }

        public static string GetDescription(this Enum value)
        {
            return GetEnumDescription(value);
        }
        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>描述信息</returns>
        private static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static T Next<T>(this T src) where T : struct, Enum
        {
            T[] arr = (T[])Enum.GetValues(typeof(T));
            int j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[j - 1] : arr[j]; // 这里处理为停留在最后一个
        }
    }
}
