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

namespace cvhdx {
    internal partial class MainForm : Form {
        public String FullName { get { return path + (path.EndsWith(@"\") ? "" : @"\") + file; } }
        public Int32 FileSize { get; private set; }

        private String path;
        private String file;

        private Int32 MAX = 67108864;
        private Dictionary<String, Int32> UNIT = new Dictionary<string, int>();

        public MainForm(string fullName) {
            InitializeComponent();

            file = Path.GetFileName(fullName);
            path = Path.GetDirectoryName(fullName);

            nameBox.Text = file;

            UNIT.Add("MB", 1);
            UNIT.Add("GB", 1024);
            UNIT.Add("TB", 1048576);
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

            var reg = new Regex(@"(^[\d]+(\.\d+){0,1})[\s]*([MGT]B)$", RegexOptions.IgnoreCase);
            var sizeText = sizeBox.Text.Trim();
            var t = reg.Matches(sizeText);
            if (t.Count != 1 || !Double.TryParse(t[0].Groups[1].Value, out Double val)) {
                MessageBox.Show("The size is not valid.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if (val * UNIT[t[0].Groups[3].Value] < 100 || MAX < val * UNIT[t[0].Groups[3].Value]) {
                MessageBox.Show("The size must be between 100MB and 64TB.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // create vdisk file="d:\test.vhdx" maximum=10240 type=expandable
            // attach vdisk
            // create partition primary
            // format fs=ntfs quick
            // https://ndswanson.wordpress.com/2014/08/12/using-diskpart-with-c/

            // Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
    }
}
