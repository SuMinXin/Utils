using System;
using System.Collections.Generic;
using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;
using Utils.Setting;

namespace Utils {
    public enum LogStatus {
        Start,
        Finished,
        Succeed,
        Failed,
        Message,
        Exception,
    }

    public class MongoLogger : IMongoLogger {
        private MongoHelper mongodb { get; set; }
        private BaseLog logger { get; set; }
        public MongoLogger(MongoHelper mongodb, string action = "") {
            this.mongodb = mongodb;
            logger = new BaseLog() { action = action };
        }
        public void setAction(string action) {
            logger.action = action;
        }
        public void setKey(string action, string logKey) {
            logger.action = action;
            logger.key = logKey;
        }
        public void setStatus(LogStatus status, string value = "") {
            logger.status = status.ToString();
            if (!string.IsNullOrWhiteSpace(value)) {
                logger.message = value;
            }
        }
        public void addEvent(LogStatus status, string value) {
            logger.status = status.ToString();
            logger.addEvent(value);
        }
        public void insert(MongoCollection collection) {
            mongodb.Insert(collection, logger, true);
        }
        #region -Simple Log-
        public void logException(string action, Exception ex) {
            logger.status = LogStatus.Exception.ToString();
            logger.message = string.Format("【{0}】 {1}", ex.Message, ex.ToString());
            mongodb.Insert(MongoCollection.Exception, logger, true);
        }
        public void logPayload(string url, Dictionary<string, string> optionHeader, object response) {
            var log = new {
                Header = optionHeader,
                RS = response
            };
            setKey("SystemPayload", url);
            setStatus(LogStatus.Message, GZip.compress(JsonSerializer.Serialize(log)));
            insert(MongoCollection.Payloads);
        }
        public void logPayload(PayloadLog log) {
            mongodb.Insert(MongoCollection.TransactionLogs, log, true);
        }
        #endregion
    }

    public class BaseLog {
        public string user { get; set; }
        public string key { get; set; }
        public string action { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public DateTime systemTime = DateTime.Now;
        [BsonIgnoreIfNull]
        public List<LogEvent> events { get; set; }
        public void addEvent(string message) {
            if (events == null) {
                events = new List<LogEvent>();
            }
            events.Add(new LogEvent(message));
        }
    }
    public class LogEvent {
        public DateTime systemTime = DateTime.Now;
        public string message { get; set; }
        public LogEvent(string message) {
            this.message = message;
        }
    }
    public class PayloadLog {
        public string authorization { get; set; }
        public string controller { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public DateTime systemTime { get; set; }
    }
}