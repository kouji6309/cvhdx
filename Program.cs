using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;

namespace cvhdx {
    internal class Program {
        public const String TITLE = "VHD Creater";

        static String SysPath = Environment.SystemDirectory;
        static String ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        static void Main(String[] args) {
            Application.EnableVisualStyles();

            if (args.Length == 1) {
                Application.Run(new MainForm(args[0]));
            } else {
                try {
                    bool isElevated = false;
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                    }
                    if (isElevated) {
                        if (!ExePath.StartsWith(SysPath)) {
                            File.Copy(ExePath, SysPath + @"\cvhdx.exe", true);
                            Registry.SetValue(@"HKEY_CLASSES_ROOT\.vhdx\ShellNew", "command", "%SystemRoot%\\system32\\cvhdx.exe \"%1\"", RegistryValueKind.ExpandString);
                            MessageBox.Show("Registered!", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    } else {
                        MessageBox.Show("Run as administrator to register.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } catch {
                    MessageBox.Show("Unable to register!", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
