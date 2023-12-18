using System.Runtime.CompilerServices;

public static class Config {
    public const string squareIcoSize = "256x256";
    public const int maxFolderPathLength = 247;
    public const int maxFilePathLength = 259;

    public static Path LibraryPath { get; private set; }
    public static Path BackupFolder { get; private set; }
    public static Path BackupPath { get; private set; }
    public static Path IcoFolder { get; private set; }
    public static bool Overwrite { get; private set; }
    public static bool Crop { get; private set; }

    private static int[] date = [
        DateTime.Now.Year,
        DateTime.Now.Month,
        DateTime.Now.Day,
        DateTime.Now.Hour,
        DateTime.Now.Minute,
        DateTime.Now.Second];
    public static int[] Date {  
        get => date;
    }
    
    public static void Init() {
        //fetch config info from .config file
    }
    /// <summary>
    /// Initializes public static fields which are used by QuickIco classes to access common runtime information
    /// </summary>
    /// <param name="debugMode"></param>
    /// <param name="_libraryPath">The full path to the media library.
    /// This should be the common parent of the directories which will actually have their icons set.</param>
    /// <exception cref="Exception"></exception>
    public static void Init(bool debugMode, string _libraryPath) {
        if (debugMode) {
            LibraryPath = new Path(_libraryPath);
            BackupFolder = new Path(@"C:\BACKUP\Software\!mine\icoSys");
            string preName = $"{date[0]}-{date[1]}-{date[2]}-{date[3]}-{date[4]}-{date[5]}";
            BackupPath = new Path(BackupFolder, $"{preName}.backup");
            IcoFolder = new Path(@"C:\BACKUP\Software\!mine\icoSys\icons");
            Overwrite = false;
            Crop = true;
        }
        else {
            throw new Exception("Release mode not yet implimented. Please run in Debug Mode");
        }
    }

}