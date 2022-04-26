using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace cvhdx {
    internal static class Program {
        [DllImport("kernel32.dll")]
        private static extern Boolean AttachConsole(Int32 dwProcessId);

        public static String Title { get; } = "VHDX Creater";

        public static String Location { get; } = Assembly.GetExecutingAssembly().Location;

        public static Boolean IsElevated { get; } = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public static Boolean Attached { get; set; } = false;

        public static MainForm MainForm { get; set; } = null;

        public static Dictionary<String, Int32> UnitMapping { get; } = new() {
            { "MB", 1 },
            { "GB", 1024 },
            { "TB", 1048576 }
        };

        [STAThread]
        private static Int32 Main(String[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Attached = AttachConsole(-1);
            var errorCode = 0;
            var message = "";
            try {
                var command = args.FirstOrDefault()?.ToLower();
                if (command == "create") {
                    DoCreate(args);
                    message = "Create successful.";
                } else if (command == "expand") {
                    DoExpand(args);
                    message = "Expand successful.";
                } else if (command == "compact") {
                    if (DoCompact(args)) {
                        message = "Compact successful.";
                    }
                } else if (IsPathValid(command) && command.ToLower().EndsWith(".vhdx")) {
                    ShowForm(args);
                } else if (!Attached) {
                    DoRegister();
                    message = "Register successful.";
                } else {
                    ShowHelp();
                }
            } catch (Exception ex) {
                message = "ERROR: " + ex.Message;
                errorCode = ex is CvhdxXException ? -2 : -1;
            }

            if (!String.IsNullOrEmpty(message)) {
                if (!Attached) {
                    Msgbox(message, MessageBoxButtons.OK, errorCode == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation);
                } else {
                    Console.WriteLine("");
                    Console.WriteLine(message);
                    Console.WriteLine("");
                    SendKeys.SendWait("{ENTER}");
                }
            }
            return errorCode;
        }

        private static void ShowHelp() {
            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine("  cvhdx <file>");
            Console.WriteLine("  cvhdx <command> <option>...");
            Console.WriteLine("");
            Console.WriteLine("  Commands:");
            Console.WriteLine("    create     Creates a virtual disk file.");
            Console.WriteLine("    expand     Expands the maximum size available on a virtual disk.");
            Console.WriteLine("    compact    Attempts to reduce the physical size of the file.");
            Console.WriteLine("");
            Console.WriteLine("  Options:");
            Console.WriteLine("    --file,       -f    Specifies the complete path and filename of the virtual disk file.");
            Console.WriteLine("    --capacity,   -c    Specifies the maximum amount of space exposed by the virtual disk, in megabytes(MB).");
            Console.WriteLine("    --initialize, -i    Create the volume and format.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("  cvhdx D:\\Database.vhdx");
            Console.WriteLine("  cvhdx create -f D:\\Database.vhdx -c 10240 -i");
            Console.WriteLine("  cvhdx expaned -f D:\\Database.vhdx -c 20480");
            Console.WriteLine("  cvhdx compact -f D:\\Database.vhdx");
            Console.WriteLine("");
            SendKeys.SendWait("{ENTER}");
        }

        private static void DoCreate(String[] args) {
            if (!IsElevated) {
                throw new CvhdxXException("Access is denied.");
            }

            if (!TryGetArg(args, new[] { "--file", "-f" }, out String file)) {
                throw new CvhdxXException("The file must be specified.");
            }

            if (!IsPathValid(file)) {
                throw new CvhdxXException("The file name is invalid.");
            }

            if (File.Exists(file)) {
                throw new CvhdxXException("The file already exists.");
            }

            if (!TryGetArg(args, new[] { "--capacity", "-c" }, out Int64 capacity)) {
                throw new CvhdxXException("The capacity must be specified.");
            }

            if (capacity < 5 || 67108864 < capacity) {
                throw new CvhdxXException("The capacity must be between 5 MB and 67108864 MB(64TB).");
            }

            TryGetArg(args, new String[] { "--initialize", "-i" }, out Boolean initialize);

            var diskFile = file.ConvertTo(Encoding.Default);
            var commands = new List<String>();
            commands.Add($"create vdisk file=\"{diskFile}\" maximum={capacity} type=expandable");
            commands.Add("attach vdisk");
            commands.Add("convert gpt");

            if (initialize) {
                commands.Add("create partition primary");
                commands.Add("format fs=ntfs quick");
                commands.Add("assign");
            }

            commands.Add("detach vdisk");
            commands.Add("exit");

            var result = RunDiskpart(commands);
            if (result.ToLower().Contains("error:")) {
                throw new CvhdxXException(result);
            }
        }

        private static void DoExpand(String[] args) {
            if (!IsElevated) {
                throw new CvhdxXException("Access is denied.");
            }

            if (!TryGetArg(args, new[] { "--file", "-f" }, out String file)) {
                throw new CvhdxXException("The file must be specified.");
            }

            if (!IsPathValid(file)) {
                throw new CvhdxXException("The file name is invalid.");
            }

            if (!File.Exists(file)) {
                throw new CvhdxXException("The file is not exists.");
            }

            if (!TryGetArg(args, new[] { "--capacity", "-c" }, out Int64 capacity)) {
                throw new CvhdxXException("The capacity must be specified");
            }

            var oldCapacity = GetVhdxCapacity(file);
            if (capacity <= oldCapacity) {
                throw new CvhdxXException("The capacity must be larger than the original capacity.");
            }

            var diskFile = file.ConvertTo(Encoding.Default);
            var commands = new[] {
                $"select vdisk file=\"{diskFile}\"",
                "detach vdisk noerr",
                $"expand vdisk maximum={capacity}",
                "exit",
            };

            var result = RunDiskpart(commands);
            if (result.ToLower().Contains("error:") && !result.ToLower().Contains("detached")) {
                throw new CvhdxXException(result);
            }
        }

        private static Boolean DoCompact(String[] args) {
            if (!IsElevated && Attached) {
                throw new CvhdxXException("Access is denied.");
            }

            if (!IsElevated) {
                try {
                    RunProcess(Location, args);
                } catch (Exception ex) {
                    Msgbox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return false;
            }

            if (!TryGetArg(args, new[] { "--file", "-f" }, out String file)) {
                throw new CvhdxXException("The file must be specified.");
            }

            if (!IsPathValid(file)) {
                throw new CvhdxXException("The file name is invalid.");
            }

            if (!File.Exists(file)) {
                throw new CvhdxXException("The file is not exists.");
            }

            var diskFile = file.ConvertTo(Encoding.Default);
            var commands = new[] {
                $"select vdisk file=\"{diskFile}\"",
                "detach vdisk noerr",
                "compact vdisk",
                "exit",
            };

            var result = RunDiskpart(commands);
            if (result.ToLower().Contains("error:") && !result.ToLower().Contains("detached")) {
                throw new CvhdxXException(result);
            }

            return true;
        }

        private static void ShowForm(String[] args) {
            try {
                var filename = args[0];
                var fileExists = File.Exists(filename);
                if (fileExists && !IsElevated) {
                    try {
                        RunProcess(Location, args);
                    } catch (Exception ex) {
                        Msgbox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (!fileExists) {
                    MainForm = new MainForm(CommandType.Create);
                    MainForm.Capacity = 102400;
                } else {
                    MainForm = new MainForm(CommandType.Expand);
                    MainForm.Capacity = MainForm.OriginalCapacity = MainForm.MinimumCapacity = GetVhdxCapacity(filename);
                }
                MainForm.Filename = filename;

                Application.Run(MainForm);
            } catch (Exception ex) {
                Msgbox(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw;
            }
        }

        private static void DoRegister() {
            if (!IsElevated) {
                Msgbox("Run as administrator to register.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new CvhdxXException("Access is denied.");
            }

            if (Location.StartsWith(Environment.SystemDirectory)) {
                return;
            }

            File.Copy(Location, Path.Combine(Environment.SystemDirectory, "cvhdx.exe"), true);
            Registry.SetValue(@"HKEY_CLASSES_ROOT\.vhdx\ShellNew", "command", "%SystemRoot%\\system32\\cvhdx.exe \"%1\"", RegistryValueKind.ExpandString);
            Registry.SetValue(@"HKEY_CLASSES_ROOT\SystemFileAssociations\.vhdx\shell\Expand\command", "", "%SystemRoot%\\system32\\cvhdx.exe \"%1\"", RegistryValueKind.ExpandString);
            Registry.SetValue(@"HKEY_CLASSES_ROOT\SystemFileAssociations\.vhdx\shell\Compact\command", "", "%SystemRoot%\\system32\\cvhdx.exe compact --file \"%1\"", RegistryValueKind.ExpandString);
        }

        public static String ConvertTo(this String data, Encoding encoding) {
            return encoding.GetString(Encoding.Convert(Encoding.UTF8, encoding, Encoding.UTF8.GetBytes(data)));
        }

        public static Boolean IsPathValid(String path) {
            return !String.IsNullOrEmpty(path) && Path.GetFileName(path).IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        }

        public static Int32 GetVhdxCapacity(String filename) {
            var diskFile = filename.ConvertTo(Encoding.Default);
            var comamnds = new[] {
                $"select vdisk file=\"{diskFile}\"",
                "detail vdisk",
                "exit"
            };
            var result = RunDiskpart(comamnds);
            var regex = new Regex("Virtual size: (.*)", RegexOptions.Multiline);
            var match = regex.Match(result);
            if (match.Success && TryParseCapacity(match.Groups[1].Value, out var capacity)) {
                return capacity;
            } else {
                throw new CvhdxXException("Unable to get VHDX capacity.");
            }
        }

        public static String RunDiskpart(IEnumerable<String> commands) {
            using var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "diskpart.exe");
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            foreach (var i in commands) {
                p.StandardInput.WriteLine(i);
            }
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd();
        }

        public static Int32 RunProcess(String filename, IEnumerable<String> args) {
            try {
                using var p = new Process();
                p.StartInfo.Verb = "runas";
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = filename;
                p.StartInfo.Arguments = String.Join(" ", args.Select(a => '"' + a + '"'));
                p.Start();
                p.WaitForExit();
                return p.ExitCode;
            } catch {
                throw;
            }
        }

        public static Boolean TryGetArg<T>(String[] args, String[] tags, out T value) {
            value = default;
            var isFlag = typeof(T) == typeof(Boolean);
            for (var i = 0; i < args.Length; i++) {
                if (!tags.Contains(args[i])) {
                    continue;
                }

                if (isFlag) {
                    value = (T)(Object)true;
                    return true;
                }

                if (i + 1 >= args.Length) {
                    return false;
                }

                try {
                    var t = TypeDescriptor.GetConverter(typeof(T));
                    value = (T)t.ConvertFrom(args[i + 1]);
                    return true;
                } catch {
                    return false;
                }
            }

            if (isFlag) {
                value = (T)(Object)false;
                return true;
            } else {
                return false;
            }
        }

        public static Boolean TryParseCapacity(String str, out Int32 capacity) {
            capacity = default;
            var reg = new Regex(@"(^[\d]+(?:\.\d+){0,1})[\s]*([MGT]B)$", RegexOptions.IgnoreCase);
            var matches = reg.Matches(str.Trim().ToUpper());
            if (matches.Count != 1 || !Double.TryParse(matches[0].Groups[1].Value, out var value)) {
                return false;
            }

            capacity = (Int32)(value * UnitMapping[matches[0].Groups[2].Value] / UnitMapping["MB"]);
            return true;
        }

        public static DialogResult Msgbox(String text, MessageBoxButtons button, MessageBoxIcon icon) {
            if (MainForm is null) {
                return MessageBox.Show(text, Title, button, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            } else {
                return MessageBox.Show(MainForm, text, Title, button, icon, MessageBoxDefaultButton.Button1);
            }
        }
    }

    internal class CvhdxXException : Exception {
        public CvhdxXException(String message) : base(message) { }
    }

    internal enum CommandType {
        Create, Expand
    }
}

