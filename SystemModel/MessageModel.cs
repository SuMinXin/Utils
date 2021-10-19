using System.Text;

namespace Utils {
    public class MessageModel {
        StringBuilder message = new StringBuilder();
        public void addMessage(string msg) {
            if (!string.IsNullOrWhiteSpace(msg)) {
                message.AppendLine(msg);
            }
        }
        public string TakeMessage() {
            string result = message.ToString();
            message.Clear();
            return result;
        }
        public string currentMessage() {
            return message.ToString();
        }
        public bool hasValue {
            get { return message.Length != 0; }
        }
        public int getLength() {
            return message.ToString().Length;
        }
        public void addLine(string msg) {
            if (!string.IsNullOrWhiteSpace(msg)) {
                message.AppendLine(msg);
            }
        }
    }
}