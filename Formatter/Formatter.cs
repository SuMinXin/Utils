using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Utils {

    /// <summary> 資料比對/驗證 </summary>
    public static class Compare {

        /// <summary> 判斷空陣列 </summary>
        public static bool EmptyCollection<T>(List<T> collection, bool removeEmpty = true) {
            if (collection == null) {
                return true;
            }
            if (removeEmpty) {
                collection.RemoveAll(o => o == null);
                collection.RemoveAll(o => string.IsNullOrWhiteSpace(o.ToString()));
            }
            return collection.Count == 0;
        }

        /// <summary> 計算陣列列數 </summary>
        public static int CollectionCount<T>(List<T> collection) {
            return collection == null ? 0 : collection.Count;
        }

        /// <summary> 拆解陣列並判斷是否包含特定字串 </summary>

        public static bool ListExist(string listString, string value) {
            List<string> list = Splitter.valueList(listString);
            return list.Contains(value);
        }

        /// <summary> 代碼比對(忽略符號&大小寫) </summary>
        public static bool Code(string value1, string value2) {
            value1 = Format.KeepAlpha(value1.ToLower());
            value2 = Format.KeepAlpha(value2.ToLower());
            return value1.Equals(value2);
        }

        /// <summary> 轉半形(忽略大小寫) </summary>
        public static bool MetaName(string value1, string value2) {
            return Format.toNarrow(value1).Equals(Format.toNarrow(value2), System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary> 判斷整數 </summary>
        public static bool isNumber(string value) {
            int i = 0;
            return int.TryParse(value, out i);
        }
    }

    /// <summary> 資料拆解/組合 </summary>
    public static class Splitter {
        /// <summary> 字串轉陣列 </summary>
        public static List<string> valueList(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return new List<string>();
            }
            return value.Split(Constants.Comma).ToList();
        }

        /// <summary> 陣列照指定數量分組 </summary>
        public static List<List<T>> groupList<T>(List<T> list, int amount) {
            list = list.Distinct().ToList();
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < list.Count; i = i + amount) {
                result.Add(list.Skip(i).Take(amount).ToList());
            }
            return result;
        }
    }

    /// <summary> 資料格式轉換 </summary>
    public static class Format {

        /// <summary> 陣列轉字串 </summary>
        public static string toString(List<string> list) {
            if (Compare.EmptyCollection(list)) {
                return string.Empty;
            }
            return string.Join(Constants.Comma, list.Distinct());
        }

        /// <summary> Dictionary轉字串 </summary>
        public static string toString(this Dictionary<string, string> dic) {
            if (dic == null || dic.Count == 0) {
                return string.Empty;
            }
            StringBuilder msg = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic) {
                msg.AppendLine($"[{item.Key}]{item.Value}");
            }
            return msg.ToString();
        }

        /// <summary> 提取字串內英數字 </summary>
        public static string KeepAlpha(string source) {
            string pattern = "[A-Za-z0-9]";
            string result = "";
            MatchCollection results = Regex.Matches(source, pattern);
            foreach (var v in results) {
                result += v.ToString();
            }
            return result;
        }

        /// <summary> 移除字串空白字元(含TAB) </summary>
        public static string removeSpace(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return null;
            }
            return value.Replace(Constants.Space, string.Empty).Replace(Constants.Tab, string.Empty);
        }

        /// <summary> 向左補 '0' </summary>
        public static string padZero(int value, int num) {
            return value.ToString().PadLeft(num, '0');
        }

        /// <summary> 取得代碼, 由 [{code}] {value}格式回推 </summary>
        public static string getDescCode(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return null;
            }
            int left = value.IndexOf(Constants.OpenBracket);
            int right = value.IndexOf(Constants.CloseBracket);
            if (left > -1 && right > -1) {
                int first = left + 1;
                return value.Substring(first, right - first);
            }
            return value;
        }

        /// <summary> 轉半形 </summary>
        public static string toNarrow(string data) {
            try {
                char[] c = data.ToCharArray();
                for (int i = 0; i < c.Length; i++) {
                    if (c[i] == 12288) {
                        c[i] = (char) 32;
                        continue;
                    }
                    if (c[i] > 65280 && c[i] < 65375)
                        c[i] = (char) (c[i] - 65248);
                }
                return new string(c);
            } catch {
                return data;
            }
        }

        /// <summary> merge valueList in Dictionary </summary>
        public static List<T> valueList<R, T>(Dictionary<R, List<T>> dic) {
            List<T> result = new List<T>();
            if (dic != null || dic.Count() > 0) {
                foreach (List<T> itemList in dic.Values) {
                    result.AddRange(itemList);
                }
            }
            return result;
        }
        /// <summary> GUID remove dash </summary>
        public static string newGuid() {
            return Guid.NewGuid().ToString().Replace(Constants.Dash, string.Empty);
        }

        /// <summary> 物件轉換 </summary>
        /// <typeparam name="T">傳入類別</typeparam>
        /// <typeparam name="R">回傳類別</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static R toParent<T, R>(T data) {
            string dataString = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<R>(dataString);
        }
    }

    /// <summary> object欄位自訂排序 </summary>
    public interface IComparerKey {
        string getComparerKey();
    }
    public class SortName : IComparer<IComparerKey> {
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(IComparerKey x, IComparerKey y) {
            if (x == null || y == null)
                throw new ArgumentException("Parameters can't be null");
            char[] arr1 = x.getComparerKey().ToCharArray();
            char[] arr2 = y.getComparerKey().ToCharArray();
            int i = 0, j = 0;
            while (i < arr1.Length && j < arr2.Length) {
                if (char.IsDigit(arr1[i]) && char.IsDigit(arr2[j])) {
                    string s1 = string.Empty, s2 = string.Empty;
                    while (i < arr1.Length && char.IsDigit(arr1[i])) {
                        s1 += arr1[i];
                        i++;
                    }
                    while (j < arr2.Length && char.IsDigit(arr2[j])) {
                        s2 += arr2[j];
                        j++;
                    }
                    if (int.Parse(s1) > int.Parse(s2)) {
                        return 1;
                    }
                    if (int.Parse(s1) < int.Parse(s2)) {
                        return -1;
                    }
                } else {
                    if (arr1[i] > arr2[j]) {
                        return 1;
                    }
                    if (arr1[i] < arr2[j]) {
                        return -1;
                    }
                    i++;
                    j++;
                }
            }
            return arr1.Length == arr2.Length ? 0 : (arr1.Length > arr2.Length ? 1 : -1);
        }
    }
}