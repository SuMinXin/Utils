namespace Utils {

    /// <summary> 試卷格式檢核 </summary>
    public static class ExamChecker {
        public static bool checkUID(string value) {
            if (!value.Contains(Constants.Dash)) {
                return false;
            }
            if (value.Length != 36) {
                return false;
            }
            string[] valueArr = value.Split(Constants.Dash);
            return valueArr[0].Length == 3 && valueArr[1].Length == 32;
        }
        public static string getYear(string value) {
            return value.Split(Constants.Dash) [0];
        }
    }
}