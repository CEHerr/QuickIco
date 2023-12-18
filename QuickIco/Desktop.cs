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
        private bool Exists(){
            return File.Exists(this);
        }

        public override string ToString() {
            return $"{path}";
        }
        public static implicit operator string(Desktop d) => d.path.path;
    }
}