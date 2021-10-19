using Utils.Setting;

namespace Utils {
    public class SystemMessage : MessageModel {
        public string systemCode { get; private set; }
        public void addException(string msg) {
            addMessage(msg);
            setCode(SystemStatus.Exception);
        }
        public void setCode(SystemStatus status, string value = "") {
            systemCode = Format.padZero((int) status, 4);
            if (!string.IsNullOrWhiteSpace(value)) {
                addLine(value);
            }
        }

        public void setNullCode(SystemStatus status, string name, string value) {
            string message = string.Format("{0} : [{1}] does not exist.", name, value);
            setCode(status, message);
        }
    }
}