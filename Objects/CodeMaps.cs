using System;

namespace Utils {

    public class CodeMap {
        public CodeMap() { }
        public CodeMap(string code, string name) {
            this.code = code;
            this.name = name;
        }
        public string code { get; set; }
        public string name { get; set; }

        // Desc => code = UID, Name = [{code}]{name}
        public string getDescCode() {
            if (name.IndexOf(Constants.CloseBracket) > 0) {
                return name.Split(Constants.CloseBracket) [0].Replace(Constants.OpenBracket, string.Empty);
            }
            return name;
        }
    }

    public class AttributeMap : CodeMap {
        public AttributeMap() { }
        public AttributeMap(string code, string name, string attrType) {
            this.code = code;
            this.name = name;
            this.type = attrType;
        }
        public string type { get; set; }
    }

    public class UIDMap : CodeMap {
        public UIDMap() { }
        public UIDMap(Guid UID, string code, string name) {
            this.UID = UID;
            this.code = code;
            this.name = name;
        }
        public Guid UID { get; set; }
    }

    public class RequiredMap : CodeMap {
        public RequiredMap() { }
        public RequiredMap(CodeMap item) {
            this.code = item.code;
            this.name = item.name;
            isRequired = true; //預設為檢核
        }
        public bool isRequired { get; set; }
    }

    public class PathMap : CodeMap {
        public string path { get; set; }
    }
}