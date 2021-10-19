using System.IO;

namespace Utils {
    public static class FileEvent {
        public enum ExtensionType {
            undefined, //default
            doc,
            docx,
            gif,
            pdf
        }

        public static ExtensionType getExtension(string file) {
            string fileExtension = Path.GetExtension(file).Replace(Constants.Dot, string.Empty);
            return ExtensionHelper.GetFromName<ExtensionType>(fileExtension);
        }

        public static bool Delete(string file) {
            if (!File.Exists(file)) {
                return true;
            }
            File.Delete(file);
            return File.Exists(file);
        }

        public static void DeleteFolder(string folder, bool doForce = true) {
            if (Directory.Exists(folder)) {
                Directory.Delete(folder, doForce);
            }
        }

        public static bool Move(string file, string targetFolder) {
            if (!File.Exists(file)) {
                return false;
            }
            string target = Path.Combine(targetFolder, Path.GetFileName(file));
            File.Move(file, target);
            File.Delete(file);
            return File.Exists(target);
        }
    }
}