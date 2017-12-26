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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace cvhdx {
    internal partial class MainForm : Form {
        private String path;
        private String file;

        public MainForm(string fullName) {
            InitializeComponent();

            file = Path.GetFileName(fullName);
            path = Path.GetDirectoryName(fullName);
            path += path.EndsWith("\\") ? "" : "\\";

            nameBox.Text = file;

        }

        private void createBtn_Click(object sender, EventArgs e) {
            file = nameBox.Text;
            if (!file.EndsWith(".vhdx")) {
                file += ".vhdx";
            }

            String errorMsg = Program.CheckName(file);
            if (!String.IsNullOrEmpty(errorMsg)) {
                nameBox.Focus();
                MessageBox.Show(errorMsg, Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            file = path + file;

            errorMsg = Program.CheckSize(sizeBox.Text, out Int32 size);
            if (!String.IsNullOrEmpty(errorMsg)) {
                sizeBox.Focus();
                MessageBox.Show(errorMsg, Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String letter = null;
            var list = new Boolean[26];
            var drives = DriveInfo.GetDrives();
            foreach (var i in drives) {
                list[i.Name.ToUpper().ToCharArray()[0] - 65] = true;
            }
            for (var i = 3; i < list.Length; i++) {
                if (!list[i]) {
                    letter = Convert.ToString((char)(i + 65));
                    break;
                }
            }

            if (String.IsNullOrEmpty(letter)) {
                MessageBox.Show("No more drive letters are avaliable.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                using (Process p = new Process()) {
                    p.StartInfo.Verb = "runas";
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = Program.EXE_PATH;
                    p.StartInfo.Arguments = "\"" + file + "\" \"" + sizeBox.Text + "\" " + letter;
                    p.Start();
                }
            } catch { }
            Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Enter:
                    createBtn_Click(null, null);
                    break;
            }
        }
    }
}
