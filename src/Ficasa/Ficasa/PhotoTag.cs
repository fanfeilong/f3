using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficasa {
    public class PhotoTag {
        public string Name {
            get;
            private set;
        }
        public PhotoTag(string name) {
            this.Name=name;
        }
        public bool IsTagName(string name) {
            return this.Name==name;
        }
    }
}
