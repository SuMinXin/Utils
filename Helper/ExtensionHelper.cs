using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Utils {
    public static class ExtensionHelper {

        /// <summary> 取得Description屬性 </summary>
        public static string GetEnumDescription(this Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            //若取不到屬性，則取名稱
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary> 名稱轉enum </summary>
        public static T GetFromName<T>(string name) {
            try {
                return (T) Enum.Parse(typeof(T), name, true);
            } catch {
                return default(T);
            }
        }

        /// <summary> 檢核名稱 </summary>
        public static bool checkName<T>(string item) {
            List<string> names = new List<string>();
            var type = typeof(T);
            if (type.IsEnum) {
                foreach (var value in Enum.GetValues(type).Cast<Enum>()) {
                    if (value.ToString().Equals("NONE", StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }
                    names.Add(value.ToString());
                }
            }
            return names.Contains(item);
        }

        /// <summary> enum -> Dictionary<value, desc> </summary>
        public static Dictionary<string, string> toValueDic<T>(bool ignoreNone = true) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var type = typeof(T);
            if (type.IsEnum) {
                foreach (var value in Enum.GetValues(type).Cast<Enum>()) {
                    if (value.ToString().Equals("NONE", StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }
                    result.Add(Convert.ToInt16(value).ToString(), ExtensionHelper.GetEnumDescription(value));
                }
            }
            return result;
        }
        /// <summary> enum -> List<enum> </summary>
        public static List<T> toList<T>(bool ignoreNone = true) {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}