using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ficasa {
    public class PhotoCollection {
        private string workspace;
        private List<Photo> photos;

        public List<Photo> Photos {
            get {
                return photos;
            }
        }

        public PhotoCollection(string workspace) {
            this.workspace=workspace;
            this.photos=new List<Photo>();
        }

        public void LoadAsync(Action callback) {
            System.Threading.ThreadPool.QueueUserWorkItem(o => {
                LoadDirectory(new DirectoryInfo(workspace));
                callback();
            });
        }

        private void LoadDirectory(DirectoryInfo directoryInfo) {
            var photoFileNames=FilteFiles(directoryInfo, "*.png|*.jpg|*.bmp",SearchOption.AllDirectories)
                .Select(f => new Photo(f.FullName));
            photos.AddRange(photoFileNames);
        }

        private IEnumerable<FileInfo> FilteFiles(DirectoryInfo directoryInfo, string Filter, SearchOption searchOption) {
            string[] MultipleFilters=Filter.Split('|');
            foreach (string FileFilter in MultipleFilters) {
                foreach (var file in directoryInfo.GetFiles(FileFilter, searchOption)) {
                    yield return file;
                }
            }
        }
    }
}
