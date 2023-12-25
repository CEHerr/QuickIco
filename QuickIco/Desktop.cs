using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static System.IO.File;
using Vanara.PInvoke;
public partial class Folder {
    /// <summary>
    /// Provied methods to access and modify Desktop.ini files which are used by Windows to store custom display options for their parent directory.
    /// </summary>
    public class Desktop {
        private const FileAttributes hidden = FileAttributes.Hidden;
        private const FileAttributes system = FileAttributes.System;

        public string path { get; }
        private Folder parent;
        private string? pathToIcon = null;
        public string? PathToIcon {
            get => pathToIcon;
            set { pathToIcon ??= value; }
        }

        public Desktop(Folder folder) {
            path = System.IO.Path.Combine(folder, "desktop.ini");
            parent = folder;
        }
        /// <summary>
        /// Output entire desktop.ini file to the console
        /// </summary>
        public void Print() {
            try {
                Console.WriteLine(ReadAllText(path));
            }
            catch {
                Console.WriteLine($"failed to read the desktop file at {path}");
            }
        }

        /// <summary>
        /// Set the icon of the parent directory to the Icon at the supplied path.
        /// </summary>
        /// <param name="icoPath">__DEPRICATED_PATH__ to the icon to be used</param>
        /// <returns>True if the operation succeeded, Else false</returns>
        public bool SetIcon(string icoPath) {
            if (icoPath is null) 
                return false;

            //this assert will fail if an invalid image makes it into the work queue
            //Debug.Assert(File.Exists(icoPath));

            var foldSet = CreateFolderSettings();
            WriteToDesktop(foldSet);
            ClearIconCache();
            return true;

            /// <summary>
            /// Creates a folder settings object which is used to determine what to write in the desktop.ini file
            /// </summary>
            Shell32.SHFOLDERCUSTOMSETTINGS CreateFolderSettings() {
                return new Shell32.SHFOLDERCUSTOMSETTINGS {
                    dwMask = Shell32.FOLDERCUSTOMSETTINGSMASK.FCSM_ICONFILE,
                    pszIconFile = icoPath,
                    dwSize = (uint)Marshal.SizeOf(typeof(Shell32.SHFOLDERCUSTOMSETTINGS)),
                };
            }
            /// <summary>
            /// Applies the folder settings object to this desktop.ini file and notifies the system of the change
            /// </summary>
            void WriteToDesktop(Shell32.SHFOLDERCUSTOMSETTINGS settings) {
                HRESULT res = Shell32.SHGetSetFolderCustomSettings
                    (ref settings
                    ,parent.Path
                    ,Shell32.FCS.FCS_FORCEWRITE);
                Shell32.SHChangeNotify
                    (Shell32.SHCNE.SHCNE_UPDATEDIR
                    ,Shell32.SHCNF.SHCNF_PATHW
                    ,parent.Path
                    ,null);
                //Console.WriteLine(res.ToString());
            }
            /// <summary>
            /// Clears the Icon Cache used by Windows Explorer in order to prevent old icons from continuing to be displayed
            /// </summary>
            void ClearIconCache() {
                string systemFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                Kernel32.Wow64DisableWow64FsRedirection(out _);
                Process clearIconCache = new Process
                {
                    StartInfo = {
                        FileName = System.IO.Path.Combine(systemFolderPath, "ie4uinit.exe"),    //ie4uinit.exe is a program that provides management functions related to the Icon Cache
                        Arguments = "-ClearIconCache",
                        WindowStyle = ProcessWindowStyle.Normal
                    }
                };
                clearIconCache.Start();
                clearIconCache.WaitForExit();
                clearIconCache.Close();
                Kernel32.Wow64EnableWow64FsRedirection(true);
            }
        }
        public override string ToString() {
            return $"{path}";
        }
        public static implicit operator string(Desktop d) => d.path;
    }
}