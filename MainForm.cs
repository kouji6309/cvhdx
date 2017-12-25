using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace cvhdx {
    internal partial class MainForm : Form {
        public String FullName { get { return path + (path.EndsWith(@"\") ? "" : @"\") + file; } }
        public UInt32 FileSize { get; private set; }

        private String path;
        private String file;

        private UInt32 MAX = 67108864;
        private UInt32[] UNIT = new UInt32[] { 1, 1024, 1048576 };

        public MainForm(string fullName) {
            InitializeComponent();

            file = Path.GetFileName(fullName);
            path = Path.GetDirectoryName(fullName);

            nameBox.Text = file;
            unitBox.SelectedIndex = 1;
        }

        private void createBtn_Click(object sender, EventArgs e) {
            file = nameBox.Text;
            if (!file.EndsWith(".vhdx")) {
                file += ".vhdx";
            }

            if (String.IsNullOrEmpty(file)) {
                nameBox.Focus();
                return;
            } else if (file.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
                MessageBox.Show(file + "\nThe file name is not valid.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if (File.Exists(FullName)) {
                MessageBox.Show(file + "\nThe file exists.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!Double.TryParse(sizeBox.Text, out Double size)) {
                MessageBox.Show("Size is not valid.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if ((FileSize = (UInt32)(size * UNIT[unitBox.SelectedIndex])) > MAX) {
                MessageBox.Show("The maximum size is 64TB.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // create vdisk file="d:\test.vhdx" maximum=10240 type=expandable
            // attach vdisk
            // create partition primary
            // format fs=ntfs quick

            Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
    }
}
