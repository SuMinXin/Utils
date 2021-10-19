using System;
using System.Collections.Generic;
using Utils.Setting;

namespace Utils {
    public interface IMongoCache {
        void insertCache(string key, object value, int expireMin);
        T getCache<T>(string key, int extendMin = 0);
        void clearCache(string key);
        void extendCache(string key, int extendMin);
    }

    public interface IMongoLogger {
        void setKey(string logKey);
        void logException(Exception ex);
        void setStatus(LogStatus status, string value = "");
        void addEvent(LogStatus status, string value);
        void insert(MongoCollection collection);
        void logPayload(string url, Dictionary<string, string> optionHeader, object response);
    }
}