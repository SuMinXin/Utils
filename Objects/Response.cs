using System;
using Utils.Setting;

namespace Utils {

    public class Response<T> {
        public string systemCode { get; set; }
        public bool isSuccess { get; set; }
        public DateTime SystemNow { get; set; }
        public string message { get; set; }
        public string disposal { get; set; }
        public T data { get; set; }
        public Response() {
            this.isSuccess = false;
        }
        public Response(string code, string message, bool keepData, T data = default(T), string disposal = "") {
            this.systemCode = code;
            this.isSuccess = code.Equals(Format.padZero((int) SystemStatus.Succeed, 4));
            this.message = message;
            if (isSuccess || keepData) {
                this.data = data;
                this.disposal = disposal;
            }
            this.SystemNow = DateTime.Now;
        }
    }

}