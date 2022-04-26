namespace cvhdx {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.actionButton = new System.Windows.Forms.Button();
            this.initializeCheck = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.originalCapacityLabel = new System.Windows.Forms.Label();
            this.filenameBox = new System.Windows.Forms.TextBox();
            this.capacityBox = new System.Windows.Forms.TextBox();
            this.capacityLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "File Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Capacity:";
            // 
            // actionButton
            // 
            this.actionButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionButton.Location = new System.Drawing.Point(9, 91);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(302, 23);
            this.actionButton.TabIndex = 5;
            this.actionButton.Text = "Create";
            this.actionButton.UseVisualStyleBackColor = true;
            this.actionButton.Click += new System.EventHandler(this.actionButton_Click);
            // 
            // initializeCheck
            // 
            this.initializeCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.initializeCheck.AutoSize = true;
            this.initializeCheck.Location = new System.Drawing.Point(243, 40);
            this.initializeCheck.Name = "initializeCheck";
            this.initializeCheck.Size = new System.Drawing.Size(63, 17);
            this.initializeCheck.TabIndex = 6;
            this.initializeCheck.Text = "Initialize";
            this.initializeCheck.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Original:";
            // 
            // originalCapacityLabel
            // 
            this.originalCapacityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.originalCapacityLabel.Location = new System.Drawing.Point(153, 65);
            this.originalCapacityLabel.Name = "originalCapacityLabel";
            this.originalCapacityLabel.Size = new System.Drawing.Size(87, 13);
            this.originalCapacityLabel.TabIndex = 10;
            this.originalCapacityLabel.Text = "102400 MB";
            this.originalCapacityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // filenameBox
            // 
            this.filenameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameBox.Location = new System.Drawing.Point(73, 13);
            this.filenameBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.filenameBox.Name = "filenameBox";
            this.filenameBox.Size = new System.Drawing.Size(239, 20);
            this.filenameBox.TabIndex = 11;
            this.filenameBox.Text = "New Hard Disk Image File.vhdx";
            this.filenameBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filenameBox_KeyUp);
            // 
            // capacityBox
            // 
            this.capacityBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.capacityBox.Location = new System.Drawing.Point(73, 39);
            this.capacityBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.capacityBox.Name = "capacityBox";
            this.capacityBox.Size = new System.Drawing.Size(78, 20);
            this.capacityBox.TabIndex = 12;
            this.capacityBox.Text = "100 GB";
            this.capacityBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.capacityBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.capacityBox_KeyUp);
            // 
            // capacityLabel
            // 
            this.capacityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.capacityLabel.Location = new System.Drawing.Point(153, 40);
            this.capacityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.capacityLabel.Name = "capacityLabel";
            this.capacityLabel.Size = new System.Drawing.Size(87, 15);
            this.capacityLabel.TabIndex = 13;
            this.capacityLabel.Text = "102400 MB";
            this.capacityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 126);
            this.Controls.Add(this.capacityLabel);
            this.Controls.Add(this.capacityBox);
            this.Controls.Add(this.filenameBox);
            this.Controls.Add(this.originalCapacityLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.initializeCheck);
            this.Controls.Add(this.actionButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create new VHDX file";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button actionButton;
        private System.Windows.Forms.CheckBox initializeCheck;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label originalCapacityLabel;
        private System.Windows.Forms.TextBox filenameBox;
        private System.Windows.Forms.TextBox capacityBox;
        private System.Windows.Forms.Label capacityLabel;
    }
}