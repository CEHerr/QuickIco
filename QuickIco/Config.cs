using PrimitiveExtensions;
using System.Text.RegularExpressions;

/// <summary>
/// Provides access to runtime information, arguments, options, and system constants
/// </summary>
public static class Config {
    //!todo change most of these to pull from a .config file
    public const string squareIcoSize = "256x256";
    //public const int maxFolderPathLength = 247;
    public const int maxFilePathLength = 259;
    private const string ext = ".ico";
    private static int maxIconPathLength;

    public static string LibraryPath { get; private set; }
    //folder path to save icons to
    public static string IcoFolder { get; private set; }

    private static bool overwrite = false;
    public static bool Overwrite { 
        get => overwrite; 
        set => overwrite = value; 
    }
    private static bool crop = false;
    public static bool Crop {
        get => crop;
        set => crop = value; 
    }

    //character to substiture for seperator chars in source image paths
    //when saving them as icons
    private static string separatorSubstitute = "$";
    public static string SeparatorSubstitute { 
        get => separatorSubstitute; 
        set => separatorSubstitute = value; 
    }
    //character to substiture for non ascii characters in source image
    //paths when saving them as icons
    private static string nonAsciiSubstitute = "_";
    public static string NonAsciiSubstitute { 
        get => nonAsciiSubstitute; 
        set => nonAsciiSubstitute = value; 
    }

    public static void Init(string libraryPath, string icoFolder) {
            LibraryPath = libraryPath;
            IcoFolder = icoFolder;
            maxIconPathLength = maxFilePathLength - (IcoFolder.Length + 1 + ext.Length);
    }
    public static void Init(string[] args) {
        LibraryPath = args[0];
        IcoFolder = args[1];
        maxIconPathLength = maxFilePathLength - (IcoFolder.Length + 1 + ext.Length);
    }

    public static string ToSaveDest(string sourceMediaPath) {
        string parent = Path.GetDirectoryName(sourceMediaPath);
        string iconRP = 
            Path.GetRelativePath(LibraryPath, parent)
            .Replace("\\", SeparatorSubstitute)
            .ReplaceNonAscii(NonAsciiSubstitute);

        if (iconRP.Length > maxIconPathLength) 
            iconRP = iconRP.Remove(maxIconPathLength);

        return Path.Combine(IcoFolder, iconRP + ext);
    }
}