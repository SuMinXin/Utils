using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using Utils.Setting;

namespace Utils {
    public class FirebaseSingleton {
        private IMongoLogger logger { get; set; }
        public FirebaseSingleton(IMongoLogger logger) {
            authProvider = new FirebaseAuthProvider(new FirebaseConfig(Connection.Decrypt.get(FirebaseInfo.WebApiKey)));
            CheckToken();
            this.logger = logger;
        }
        // ======================= Login ======================= 
        private FirebaseAuthLink auth { get; set; }
        private FirebaseAuthProvider authProvider { get; set; }
        //======================================================
        private static readonly object firebaselock = new object();
        private static FirebaseSingleton _instance = null;
        public static FirebaseSingleton Instance(IMongoLogger logger) {
            lock(firebaselock) {
                if (_instance == null) {
                    _instance = new FirebaseSingleton(logger);
                }
            }
            return _instance;
        }

        public bool getDoc(string fileUrl, string filePath) {
            //download form firebase
            bool gotFile = Task.Run(() => download(fileUrl, filePath)).Result;
            if (gotFile) {
                return true;
            }
            return false;
        }

        public async Task<bool> download(string downloadURL, string destPath) {
            try {
                CheckToken();
                using(var client = new HttpClient()) {
                    var response = Task.Run(() => client.GetAsync(new Uri(downloadURL))).Result;
                    if (response.IsSuccessStatusCode) {
                        using(var stream = await response.Content.ReadAsStreamAsync()) {
                            var fileInfo = new FileInfo(destPath);
                            using(var fileStream = fileInfo.OpenWrite()) {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
                return File.Exists(destPath);
            } catch {
                return false;
            }
        }

        public async Task<string> upload(string file, string targetFolder) {
            string result = string.Empty;
            try {
                if (!File.Exists(file)) {
                    return result;
                }
                CheckToken();
                using(Stream stream = File.Open(file, FileMode.Open)) {
                    string fileName = Path.GetFileName(file);
                    var task = new FirebaseStorage(Connection.Decrypt.get(FirebaseInfo.Project) + ".appspot.com",
                            new FirebaseStorageOptions() {
                                AuthTokenAsyncFactory = () => Task.Run(() => auth.FirebaseToken),
                                    ThrowOnCancel = true
                            })
                        .Child(targetFolder)
                        .Child(fileName)
                        .PutAsync(stream);

                    // Track progress of the upload
                    task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
                    result = await task;
                }
            } catch (Exception ex) {
                logger.logException("UploadFirebase", ex);
            }
            return result;
        }

        private void CheckToken() {
            if (auth == null) {
                auth = Task.Run(() => authProvider.SignInAnonymouslyAsync().Result).Result;
            }
            if (auth.IsExpired()) {
                auth = Task.Run(() => authProvider.RefreshAuthAsync(auth)).Result;
            }
        }
    }

}