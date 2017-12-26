using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace cvhdx {
    internal class Program {
        public static String TITLE = "VHD Creater";
        public static Int32 MAX_SIZE = 67108864;
        public static String SYS_PATH = Environment.SystemDirectory;
        public static String EXE_PATH = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static Dictionary<String, Int32> UNIT = new Dictionary<string, int>();

        [STAThread]
        static void Main(String[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UNIT.Add("MB", 1);
            UNIT.Add("GB", 1024);
            UNIT.Add("TB", 1048576);

            bool isElevated = false;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (args.Length == 1) {
                Application.Run(new MainForm(args[0]));
            } else if (args.Length == 3 && isElevated) {
                try {
                    var letterText = args[2].ToUpper()[0].ToString();

                    String errorMsg = CheckName(args[0]);
                    if (!String.IsNullOrEmpty(errorMsg)) {
                        Console.WriteLine("ERROR: " + errorMsg);
                        return;
                    }

                    errorMsg = CheckSize(args[1], out Int32 size);
                    if (!String.IsNullOrEmpty(errorMsg)) {
                        Console.WriteLine("ERROR: " + errorMsg);
                        return;
                    }
                    var path = Path.GetDirectoryName(args[0]);
                    path += path.EndsWith("\\") ? "" : "\\";

                    var tempName = path + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".vhdx"; ;

                    using (Process p = new Process()) {
                        p.StartInfo.Verb = "runas";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = Program.SYS_PATH + @"\diskpart.exe";
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.CreateNoWindow = true;
                        p.Start();
                        p.StandardInput.WriteLine("create vdisk file=\"" + tempName + "\" maximum=" + size + " type=expandable");
                        p.StandardInput.WriteLine("attach vdisk");
                        p.StandardInput.WriteLine("convert gpt");
                        p.StandardInput.WriteLine("create partition primary");
                        p.StandardInput.WriteLine("format fs=ntfs quick");
                        p.StandardInput.WriteLine("assign letter=" + letterText);
                        p.StandardInput.WriteLine("detach vdisk");
                        p.StandardInput.WriteLine("exit");
                        p.WaitForExit();
                    }

                    File.Move(tempName, args[0]);
                } catch (Exception ex) {
                    MessageBox.Show("Unable to create VHDX.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                try {
                    if (isElevated) {
                        if (!EXE_PATH.StartsWith(SYS_PATH)) {
                            File.Copy(EXE_PATH, SYS_PATH + @"\cvhdx.exe", true);
                            Registry.SetValue(@"HKEY_CLASSES_ROOT\.vhdx\ShellNew", "command", "%SystemRoot%\\system32\\cvhdx.exe \"%1\"", RegistryValueKind.ExpandString);
                            MessageBox.Show("Registered!", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    } else {
                        MessageBox.Show("Run as administrator to register.", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } catch {
                    MessageBox.Show("Unable to register!", Program.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static String CheckName(String file) {
            if (string.IsNullOrEmpty(file)) {
                return "You must type a file name.";
            } else if (Path.GetFileName(file).IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
                return "The file name is not valid.";
            } else if (File.Exists(file)) {
                return "The file exists.";
            }
            return null;
        }

        public static String CheckSize(String size, out Int32 FileSize) {
            var reg = new Regex(@"(^[\d]+(\.\d+){0,1})[\s]*([MGT]B)$", RegexOptions.IgnoreCase);
            var sizeText = size.Trim();
            var t = reg.Matches(sizeText);
            FileSize = -1;
            var temp = 0.0;
            if (t.Count != 1 || !Double.TryParse(t[0].Groups[1].Value, out Double val)) {
                return "The size is not valid.";
            } else if ((FileSize = (Int32)(val * UNIT[t[0].Groups[3].Value])) < 100 || Program.MAX_SIZE < temp) {
                return "The size must be between 100MB and 64TB.";
            }
            return null;
        }

    }
}
