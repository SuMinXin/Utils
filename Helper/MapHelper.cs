using System;
using System.Collections.Generic;
using System.Linq;

/// <summary> For object:CodeMap </summary>
namespace Utils {
    public static class MapFormat {
        /// <summary> 過濾重複資料 (CodeMap陣列使用) </summary>
        public static List<CodeMap> doDistinct(this List<CodeMap> list, bool upper = true) {
            if (upper) {
                list.ForEach(o => o.code = o.code.ToUpper());
            }
            return (from m in list group m by new { m.code } into g select g.FirstOrDefault()).Distinct().ToList();
        }

        /// <summary> CodeMap 轉 Dictionary (code as Key) </summary>
        public static Dictionary<string, string> toCodeDic(this List<CodeMap> list) {
            return list.ToDictionary(o => o.code, o => o.name);
        }

        /// <summary> CodeMap 轉 Dictionary (name as Key) </summary>
        public static Dictionary<string, string> toNameDic(this List<CodeMap> list) {
            return list.ToDictionary(o => o.name, o => o.code);
        }

        /// <summary> 產生數字選單 </summary>
        public static List<CodeMap> numSelection(int max, int min = 1) {
            List<CodeMap> res = new List<CodeMap>();
            for (int i = 0; i < max; i++) {
                string item = (i + 1).ToString();
                res.Add(new CodeMap(item, item));
            }
            return res;
        }

        /// <summary> CodeMap陣列轉字串 </summary>
        public static string name(this List<CodeMap> list) {
            if (Compare.EmptyCollection(list)) {
                return string.Empty;
            }
            List<string> values = list.Select(o => o.name).ToList();
            return Format.toString(values);
        }

        /// <summary> enum 轉 CodeMap </summary>
        public static List<CodeMap> toCodeMap<T>(bool ignoreNone = true) {
            List<CodeMap> maps = new List<CodeMap>();
            var type = typeof(T);
            if (type.IsEnum) {
                foreach (var value in Enum.GetValues(type).Cast<Enum>()) {
                    if (value.ToString().Equals("NONE", StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }
                    maps.Add(new CodeMap(value.ToString(), ExtensionHelper.GetEnumDescription(value)));
                }
            }
            return maps;
        }

        /// <summary>只保留指定選項</summary>
        public static List<CodeMap> filterMap(IEnumerable<string> values, List<CodeMap> list) {
            values = values.Distinct();
            return list.Where(o => values.Contains(o.code)).ToList(); // 只留有資料的選項
        }

        /// <summary> get Name by code</summary>
        public static string findName(this List<CodeMap> list, string code) {
            if (code == null) {
                return string.Empty;
            }
            CodeMap item = list.Find(o => o.code.Equals(code));
            return item == null ? string.Empty : item.name;
        }
    }
}