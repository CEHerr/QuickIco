using PrimitiveExtensions;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vanara.Extensions.Reflection;

/// <summary>
/// Provides access to runtime information, arguments, options, and system constants
/// </summary>
public static class Config {
    public const string squareIcoSize = "256x256";
    public const int maxFolderPathLength = 247;
    public const int maxFilePathLength = 259;
    private const string ext = ".ico";

    public static __DEPRICATED_PATH__ LibraryPath { get; private set; }
    public static __DEPRICATED_PATH__ BackupFolder { get; private set; }
    public static __DEPRICATED_PATH__ BackupPath { get; private set; }
    public static __DEPRICATED_PATH__ IcoFolder { get; private set; }
    public static bool Overwrite { get; private set; }
    public static bool Crop { get; private set; }
    public static string SeparatorSubstitute { get; private set; }
    public static string NonAsciiSubstitute { get; private set; }

    private static int[] _date = [
        DateTime.Now.Year,
        DateTime.Now.Month,
        DateTime.Now.Day,
        DateTime.Now.Hour,
        DateTime.Now.Minute,
        DateTime.Now.Second];
    private static DateTime date = DateTime.Now;
    public static DateTime Date { get; }
    
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
            LibraryPath = new __DEPRICATED_PATH__(_libraryPath);
            BackupFolder = new __DEPRICATED_PATH__(@"C:\BACKUP\Software\!mine\icoSys");
            string preName = 
                $"{date.Year}-{date.Month}-{date.Day}-{date.Hour}-{date.Minute}-{date.Second}";
            BackupPath = new __DEPRICATED_PATH__(BackupFolder, $"{preName}.backup");
            IcoFolder = new __DEPRICATED_PATH__(@"C:\BACKUP\Software\!mine\icoSys\icons");
            Overwrite = false;
            Crop = true;
            SeparatorSubstitute = "$";
            NonAsciiSubstitute = "_";
        }
        else {
            throw new Exception("Release mode not yet implimented. Please run in Debug Mode");
        }
    }
    public static void Init(string[] args) {

    }

    public static __DEPRICATED_PATH__ ToSaveDest(__DEPRICATED_PATH__ sourceMediaPath) {
        string parent =
            Regex.Match(sourceMediaPath, @"^.*(?=\\)")
            .Value;
        string iconRP = 
            __DEPRICATED_PATH__.GetRelativePath(parent)
            .Replace("\\", SeparatorSubstitute)
            .ReplaceNonAscii(NonAsciiSubstitute);
        bool pathTooLong = 
            (iconRP.Length + IcoFolder.path.Length + 1)
            > (maxFilePathLength - 4);
        
        if (pathTooLong) iconRP = Trim(iconRP);

        return System.IO.Path.Combine(IcoFolder, iconRP + ext);

        string Trim(string str) {
            return str.Remove(
                maxFilePathLength - (
                IcoFolder.path.Length + 1) -
                ext.Length);
        }
        //Note on unique key: two files will not have the same creation time
    }
}