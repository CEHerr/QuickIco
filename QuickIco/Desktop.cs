using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static System.IO.File;
public partial class Folder {
    /// <summary>
    /// Provied methods to access and modify Desktop.ini files which are used by Windows to store custom display options for their parent directory.
    /// </summary>
    public class Desktop {
        private const FileAttributes hidden = FileAttributes.Hidden;
        private const FileAttributes system = FileAttributes.System;

        public Path path { get; }
        private Folder parent;
        private Path? pathToIcon = null;
        public Path? PathToIcon {
            get => pathToIcon;
            set { pathToIcon ??= value; }
        }

        public Desktop(Folder folder) {
            path = new Path(folder, "desktop.ini");
            parent = folder;
        }
        //output to Console
        public void Print() {
            try {
                using (StreamReader sr = new (path)) {
                    string? line;
                    while ((line = sr.ReadLine()) is not null) {
                        Console.WriteLine(line);
                    }
                }
            }
            catch {
                Console.WriteLine($"failed to read the desktop file at {path}");
            }
        }
        //sets this desktop.ini's iconResource to point to the default icon location for this folder
        public void UpdateIcon() {
            if (this.Exists()) {
                SetAttributes(
                    this,
                    GetAttributes(this) & ~(hidden | system));
            }

            WriteAllText(this, BuildDesktop());

            SetAttributes(
                this,
                GetAttributes(this) | hidden | system);
            SetAttributes(
                parent.path,
                GetAttributes(parent.path) | system);
        }
        //returns the contents of a new desktop.ini file for writing
        private string BuildDesktop() {
            string ip = parent.GetIconPath();
            StringBuilder sb = new();
            sb.AppendLine("[.ShellClassInfo]");
            sb.AppendLine("IconResource=" + ip + ",0");
            sb.AppendLine("IconFile=" + ip);
            sb.AppendLine("IconIndex=0");
            return sb.ToString();
        }

        /// <summary>
        /// Set the icon of the parent directory to the Icon at the supplied path.
        /// During normal operation simply call with this.SetIcon(this.pathToIcon);
        /// </summary>
        /// <param name="icoPath">Path to the icon to be used</param>
        /// <returns>True if the operation succeeded, Else false</returns>
        public bool SetIcon(Path icoPath) {
            if (icoPath is null) { return false; }
            var foldSet = CreateFolderSettings();
            WriteToDesktop(foldSet);
            ClearIconCache();
            return true;


            Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS CreateFolderSettings() {
                return new Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS {
                    dwMask = Vanara.PInvoke.Shell32.FOLDERCUSTOMSETTINGSMASK.FCSM_ICONFILE,
                    pszIconFile = icoPath,
                    dwSize = (uint)Marshal.SizeOf(typeof(Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS)),
                };
            }

            void WriteToDesktop(Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS settings) {
                Vanara.PInvoke.Shell32.SHGetSetFolderCustomSettings
                    (ref settings
                    ,parent.path
                    ,Vanara.PInvoke.Shell32.FCS.FCS_FORCEWRITE);
                Vanara.PInvoke.Shell32.SHChangeNotify
                    (Vanara.PInvoke.Shell32.SHCNE.SHCNE_UPDATEDIR
                    ,Vanara.PInvoke.Shell32.SHCNF.SHCNF_PATHW
                    ,parent.path
                    ,null);
            }

            void ClearIconCache() {
                string systemFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                Vanara.PInvoke.Kernel32.Wow64DisableWow64FsRedirection(out _);
                Process clearIconCache = new Process {
                    StartInfo = {
                        FileName = System.IO.Path.Combine(systemFolderPath, "ie4uinit.exe"),
                        Arguments = "-ClearIconCache",
                        WindowStyle = ProcessWindowStyle.Normal
                    }
                };
                clearIconCache.Start();
                clearIconCache.WaitForExit();
                clearIconCache.Close();
                Vanara.PInvoke.Kernel32.Wow64EnableWow64FsRedirection(true);
            }
        }


        private bool Exists(){
            return File.Exists(this);
        }

        public override string ToString() {
            return $"{path}";
        }
        public static implicit operator string(Desktop d) => d.path.path;
    }
}