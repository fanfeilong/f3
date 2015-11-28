using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficasa {
    public class PhotoManager {
        private PhotoCollection photoCollection;

        public List<Photo> Photos {
            get {
                return photoCollection.Photos;
            }
        }

        public PhotoManager() {

        }

        public void SetWorkspace(string workspace) {
            photoCollection=new PhotoCollection(workspace);
        }

        public void LoadAsync(Action callback) {
            photoCollection.LoadAsync(callback);
        }

        
    }
}
