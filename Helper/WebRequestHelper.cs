using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace Utils {
    public enum HttpMethod {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class WebRequestHelper {
        private IMongoLogger logger { get; set; }
        public WebRequestHelper(IMongoLogger logger) {
            this.logger = logger;
        }

        private const string json = "application/json";

        /// <summary>送出API請求</summary>
        public string sendAPI(HttpMethod method, string url, Dictionary<string, string> optionHeader = null, object postdata = null, bool waitResponse = true, string contentType = json) {
            ApiPara para = new ApiPara() {
            method = method.ToString(),
            url = url,
            optionHeader = optionHeader,
            contentType = contentType
            };
            switch (method) {
                case HttpMethod.GET:
                    return GetApi(para);
                case HttpMethod.POST:
                case HttpMethod.PUT:
                case HttpMethod.DELETE:
                    para.setObject(postdata);
                    if (!waitResponse) {
                        WithoutResponse(para);
                        return string.Empty;
                    }
                    return FormApi(para);
                default:
                    logger.logException(new Exception($"HttpMethod: {method} does not exist."));
                    return string.Empty;
            }
        }

        /// <summary>[GET]送出API請求</summary>
        private string GetApi(ApiPara para) {
            try {
                HttpWebRequest request = defaultHttp(para);
                return send(request);
            } catch (Exception ex) {
                logger.logException(ex);
            }
            return string.Empty;
        }

        /// <summary>送出API請求</summary>
        private string FormApi(ApiPara para) {
            try {
                HttpWebRequest request = defaultHttp(para);
                using(Stream req = request.GetRequestStream()) {
                    req.Write(para.bs, 0, para.bs.Length);
                }
                return send(request);
            } catch (Exception ex) {
                logger.logException(ex);
            }
            return string.Empty;
        }

        private string send(HttpWebRequest request) {
            try {
                string ret = string.Empty;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    StreamReader tReader = new StreamReader(response.GetResponseStream());
                    ret = tReader.ReadToEnd();
                    tReader.Close();
                }
                return ret;
            } catch (WebException wex) {
                using(HttpWebResponse response = (HttpWebResponse) wex.Response) {
                    using(StreamReader tReader = new StreamReader(response.GetResponseStream())) {
                        var errorMsg = tReader.ReadToEnd();
                        return errorMsg;
                    }
                }
            }
        }

        /// <summary>[送出API請求</summary>
        private void WithoutResponse(ApiPara para) {
            try {
                HttpWebRequest request = defaultHttp(para);
                using(Stream req = request.GetRequestStream()) {
                    req.Write(para.bs, 0, para.bs.Length);
                }
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    /// discard response
                }
            } catch (Exception ex) {
                logger.logException(ex);
            }
        }

        private HttpWebRequest defaultHttp(ApiPara para) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(para.url);
            request.ContentType = para.contentType;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = para.method;
            request.Timeout = 300000; //單位：毫秒，目前設定為5分鐘
            if (para.hasPostData()) { request.ContentLength = para.bs.Length; }
            if (para.hasHeader()) {
                foreach (string key in para.optionHeader.Keys) {
                    request.Headers.Add(key, para.optionHeader[key]);
                }
            }
            return request;
        }

        class ApiPara {
            public string method { get; set; }
            public string url { get; set; }
            public byte[] bs { get; private set; }
            public string postdata { get; private set; }
            public Dictionary<string, string> optionHeader { get; set; }
            public string contentType { get; set; }
            public void setObject(object data) {
                postdata = JsonSerializer.Serialize(data);
                bs = System.Text.Encoding.UTF8.GetBytes(postdata);
            }
            public bool hasPostData() {
                return !string.IsNullOrWhiteSpace(postdata);
            }
            public bool hasHeader() {
                return optionHeader != null;
            }
        }
    }
}