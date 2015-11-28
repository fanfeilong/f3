using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ficasa {
    public class Photo {
        private string path;
        private string name;
        private string extension;
        private List<PhotoTag> tags;

        public string Name {
            get {
                return name;
            }
            set {
                name=value;
            }
        }
        public string FullName {
            get {
                return Path.Combine(path, name+extension);
            }
        }

        public List<PhotoTag> Tags {
            get {
                return tags;
            }
        }

        public Photo(string fullName) {
            this.name=Path.GetFileNameWithoutExtension(fullName);
            this.extension=fullName.Substring(fullName.LastIndexOf("."));
            this.path=Path.GetDirectoryName(fullName);
            this.tags=new List<PhotoTag>();
        }

        public void Delete() {
            var deleteTagName = "delete";
            if (this.tags.Any(t => t.IsTagName(deleteTagName))) {
                return;
            } else {
                this.tags.Add(new PhotoTag(deleteTagName));
            }
        }
    }
}
