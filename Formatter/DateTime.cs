using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Utils {

    /// <summary> 日期格式 </summary>
    public enum DateFormat {
        [Description(@"yyyy-MM-dd'T'HH:mm:ss")]
        General, [Description(@"yyyyMMdd")]
        Char8, [Description(@"yyyy/MM/dd")]
        WithSlash, [Description(@"yyyy-MM-dd")]
        WithDash, [Description(@"yyyyMM")]
        YearMonth
    }

    /// <summary> 時間單位 </summary>
    public enum TimeType {
        None,
        [Description(@"年")]
        Year,
        [Description(@"月")]
        Month,
        [Description(@"天")]
        Day,
        [Description(@"小時")]
        Hour
    }

    public static class DateTimes {
        /// <summary> 檢核時間格式 </summary>
        public static bool check(string value, DateFormat format, bool allowEmpty = false) {
            if (allowEmpty && string.IsNullOrWhiteSpace(value)) {
                return true;
            }
            try {
                DateTime.ParseExact(value, format.GetEnumDescription(), CultureInfo.InvariantCulture);
                return true;
            } catch {
                return false;
            }
        }

        /// <summary> 字串轉時間 </summary>
        public static DateTime toDate(string value, DateFormat format, DateTime defaultDate) {
            if (check(value, format)) {
                return DateTime.ParseExact(value, format.GetEnumDescription(), CultureInfo.InvariantCulture);
            }
            return defaultDate;
        }

        /// <summary> 時間戳轉換</summary>
        public static long toTimestamp(this DateTime time) {

            return new DateTimeOffset(time.ToUniversalTime()).ToUnixTimeSeconds();;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary> 檢核起訖日 </summary>
        public static string checkStartEnd(DateTime start, DateTime end, int dayLimit) {
            string message = string.Empty;
            if (start > end) {
                message = Message.StartEnd;
            }
            double range = new TimeSpan(end.Ticks - start.Ticks).TotalDays;
            if (range > dayLimit) {
                message = DateTimes.Message.RangeLimit(TimeType.Day, dayLimit);
            }
            return message;
        }

        /// <summary> 系統年月格式 (yyyyMM) </summary>
        public static string yearMonth() {
            return DateTime.Now.toYearMonth();
        }
        /// <summary> 系統utc格式 </summary>
        public static string utcNow() {
            string format = DateFormat.General.GetEnumDescription();
            return DateTime.UtcNow.ToString(format);
        }
        /// <summary> 計算年月區間 </summary>
        public static List<string> yearMonthRange(DateTime start, DateTime end) {
            if (start.intYearMonth().Equals(end.intYearMonth())) {
                return new List<string>() { start.toYearMonth() };
            }
            List<string> result = new List<string>();
            do {
                result.Add(start.toYearMonth());
                start = start.AddMonths(1);
            } while (start.intYearMonth() <= end.intYearMonth());
            return result;
        }
        /// <summary> 時間轉年月格式 </summary>
        public static string toYearMonth(this DateTime time) {
            return time.ToString(DateFormat.YearMonth.GetEnumDescription());
        }
        public static int intYearMonth(this DateTime time) {
            return Convert.ToInt32(time.ToString(DateFormat.YearMonth.GetEnumDescription()));
        }
        /// <summary> SchoolYear </summary>
        public static string currentSchoolYear() {
            //當年的九月開學到次年六月結束
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            int year = taiwanCalendar.GetYear(DateTime.Now);
            if (DateTime.Now.Month < 7) {
                return (year - 1).ToString();
            }
            return year.ToString();
        }

        #region -Message-
        public static class Message {
            public static string Format(string name, DateFormat type) {
                return $"{name} 時間格式有誤:{type.GetEnumDescription()}";
            }
            public static readonly string StartEnd = @"起始日期不可大於結束日期";
            public static string RangeLimit(TimeType type, int value) {
                string result = $"時間區間大於系統設定";
                if (!type.Equals(TimeType.None)) {
                    result += $":{value}{type.GetEnumDescription()}";
                }
                return result;
            }
            #endregion
        }
    }
}