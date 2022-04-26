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
using System.Diagnostics;

namespace cvhdx {
    internal partial class MainForm : Form {

        private readonly ToolTip capacityToolTip = new();
        private readonly ToolTip filenameToolTip = new();

        private CommandType type;
        public MainForm(CommandType type) : base() {
            InitializeComponent();

            capacityBox.KeyUp += checkActionButton;
            filenameBox.KeyUp += checkActionButton;

            this.type = type;
            if (type == CommandType.Create) {
                label3.Visible = false;
                originalCapacityLabel.Visible = false;
            }
            if (type == CommandType.Expand) {
                filenameBox.Enabled = false;
                initializeCheck.Enabled = false;
                actionButton.Text = "Expand";
                Text = "Expand the VHDX file";
            }
        }

        private Int32 originalCapacity = -1;
        public Int32 OriginalCapacity {
            get { return originalCapacity; }
            set {
                originalCapacity = value;
                originalCapacityLabel.Text = originalCapacity + " MB";
            }
        }

        private Boolean isCapacityValid = false;
        private Int32 _capacity = -1;
        private Int32 capacity {
            get { return _capacity; }
            set {
                _capacity = value;
                if (_capacity > 0) {
                    capacityLabel.Text = _capacity + " MB";
                } else {
                    capacityLabel.Text = "-";
                }
            }
        }
        public Int32 Capacity {
            get { return capacity; }
            set {
                capacity = value;
                capacityBox.Text = capacity + " MB";
                capacityBox_KeyUp(null, null);
            }
        }

        private Int32 minimumCapacity { get; set; } = 5;
        public Int32 MinimumCapacity {
            get { return minimumCapacity; }
            set {
                minimumCapacity = value;
                capacityBox_KeyUp(null, null);
            }
        }

        private void capacityBox_KeyUp(object sender, KeyEventArgs e) {
            isCapacityValid = false;
            capacityToolTip.RemoveAll();

            if (!Program.TryParseCapacity(capacityBox.Text, out var value)) {
                capacityBox.ForeColor = Color.Red;
                capacityToolTip.SetToolTip(capacityBox, "The capacity vaue is invalid.");
                capacity = -1;
                return;
            }

            if (value < minimumCapacity || 67108864 < value) {
                var minText = minimumCapacity + "MB";
                capacityBox.ForeColor = Color.Red;
                capacityToolTip.SetToolTip(capacityBox, "The capacity must be between " + minText + " and 64TB.");
                capacity = -1;
                return;
            }

            capacityBox.ForeColor = SystemColors.WindowText;
            capacity = (Int32)value;
            isCapacityValid = true;
        }

        private String _basepath = "";
        private String _filename = "";
        public String Filename {
            get {
                var filename = _filename;
                if (!filename.ToLower().EndsWith(".vhdx")) {
                    filename = Path.ChangeExtension(filename, ".vhdx");
                }
                return Path.Combine(_basepath, filename);
            }
            set {
                try {
                    _basepath = Path.GetDirectoryName(value);
                    _filename = Path.GetFileName(value);
                    filenameBox.Text = _filename;
                    filenameBox_KeyUp(null, null);
                } catch { }
            }
        }

        private Boolean isFilenameValid = false;
        private void filenameBox_KeyUp(object sender, KeyEventArgs e) {
            isFilenameValid = false;
            filenameToolTip.RemoveAll();
            var filename = filenameBox.Text;

            if (String.IsNullOrEmpty(filename)) {
                filenameBox.ForeColor = Color.Red;
                filenameToolTip.SetToolTip(filenameBox, "You must type a file name.");
                return;
            }

            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) > 0) {
                filenameBox.ForeColor = Color.Red;
                filenameToolTip.SetToolTip(filenameBox, "The file name is invalid.");
                return;
            }

            filenameBox.ForeColor = SystemColors.WindowText;
            _filename = filename;
            isFilenameValid = true;
        }

        private void checkActionButton(object sender, KeyEventArgs e) {
            actionButton.Enabled = isFilenameValid && isCapacityValid;
        }

        private void actionButton_Click(object sender, EventArgs e) {
            var command = new List<String>();
            if (type == CommandType.Create) {
                command.Add("create");
                if (initializeCheck.Checked) {
                    command.Add("--initialize");
                }
            }
            if (type == CommandType.Expand) {
                command.Add("expand");
            }
            command.Add("--file");
            command.Add(Filename);
            command.Add("--capacity");
            command.Add(Capacity.ToString());

            try {
                var result = Program.RunProcess(Program.Location, command);
                if (result != 0) {
                    Program.Msgbox("Unable to create VHDX.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            } catch (Exception ex) {
                Program.Msgbox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            Close();
        }
    }
}
