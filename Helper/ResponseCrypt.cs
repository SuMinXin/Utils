using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utils {
    public static class AesCrypt {

        /// <summary> 加密 </summary>
        /// <param name="original">原始字串</param>
        /// <param name="key">自訂金鑰</param>
        /// <param name="iv">自訂向量</param>
        public static string AesEncrypt(string original, string key, string iv) {
            string encrypt = "";
            try {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] keyData = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                byte[] ivData = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));
                byte[] dataByteArray = Encoding.UTF8.GetBytes(original);

                using(MemoryStream ms = new MemoryStream()) {
                    using(
                        CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(keyData, ivData), CryptoStreamMode.Write)
                    ) {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(ms.ToArray());
                    }
                }
            } catch {
                //ignore
            }
            return encrypt;
        }

        /// <summary> 解密 </summary>
        /// <param name="original">原始字串</param>
        /// <param name="key">自訂金鑰</param>
        /// <param name="iv">自訂向量</param>
        public static string AesDecrypt(string data, string key, string iv) {
            string decrypt = "";
            try {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] keyData = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                byte[] ivData = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));
                aes.Key = keyData;
                aes.IV = ivData;

                byte[] dataByteArray = Convert.FromBase64String(data);
                using(MemoryStream ms = new MemoryStream()) {
                    using(CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return decrypt;
            } catch {
                throw new Exception("AesDecrypt Failed.");
            }
        }
    }
}