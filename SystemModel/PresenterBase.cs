using Utils.Setting;

namespace Utils {
    public class PresenterBase {
        public PresenterBase() {
            keepData = false;
        }
        protected UserProfile user { get; private set; }
        protected SystemMessage message = new SystemMessage();
        private bool keepData { get; set; }
        private string disposal { get; set; }
        public object res { get; protected set; }
        public Response<object> response {
            get { return new Response<object>(message.systemCode, message.TakeMessage(), keepData, res, disposal); }
        }
        protected void keepResponse() {
            keepData = true;
        }
        protected void setDisposal(string key) {
            this.disposal = key;
        }
        protected void setUser(UserProfile user) {
            this.user = user;
            message.setCode(SystemStatus.Start);
        }
    }
}