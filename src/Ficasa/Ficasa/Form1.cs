using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Ficasa {
    public partial class Form1 : Form {
        private PhotoManager photoManager;
        private bool loading;

        public Form1() {
            InitializeComponent();
            InitializeViewModle();
        }

        private void InitializeViewModle(){
            loading=true;
            photoManager=new PhotoManager();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            var folderBrowserDialog=new FolderBrowserDialog();

            var workspace = @"C:\Users\ffl\Desktop\wanaiping";
            //var workspace=@"C:\Users\ffl\OneDrive\Document\images";
            if (Directory.Exists(workspace)) {
                folderBrowserDialog.SelectedPath=workspace;
            } else {
                folderBrowserDialog.SelectedPath=@"C:\Users\ffl\Desktop\";
            }
              
            var result = folderBrowserDialog.ShowDialog();

            if (result==DialogResult.OK) {
                photoManager.SetWorkspace(folderBrowserDialog.SelectedPath);
                photoManager.LoadAsync(() => {
                    var photos=photoManager.Photos;
                    if (photos.Any()) {
                        this.Invoke(new Action(() => {
                            this.listBox1.BeginUpdate();
                            this.listBox1.DataSource=null;
                            this.listBox1.DataSource=photos;
                            this.listBox1.DisplayMember="Name";
                            this.listBox1.ValueMember="FullName";
                            this.listBox1.EndUpdate();
                            this.loading=false;

                            var firstPhoto=photos[0].FullName;
                            var bmp=Image.FromFile(firstPhoto);
                            this.pictureBox1.Image=bmp;
                            this.Text="Ficasa  "+firstPhoto;
                        }));
                    } else {
                        // Ignore                    
                    }
                });
            } else {
                // Ignore
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e) {
            // TODO
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.loading==false) {
                var photo=listBox1.SelectedItem as Photo;
                this.pictureBox1.Image=Image.FromFile(photo.FullName);
                this.Text="Ficasa  "+photo.FullName;
            }
        }
    }
}
