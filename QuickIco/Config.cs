using PrimitiveExtensions;
using System.Text.RegularExpressions;

/// <summary>
/// Provides access to runtime information, arguments, options, and system constants
/// </summary>
public static class Config {
    //todo: change most of these to pull from a .config file
    /// <summary>Dimensions to use for cropped icons</summary>
    public const string squareIconSize = "256x256";
    public const int maxFilePathLength = 259;
    private const string ext = ".ico";
    /// <summary>
    /// The maximum RELATIVE path that an icon file may have considering the length of the path to
    /// the containing icon folder and the length of the .ico extension
    /// </summary>
    private static int maxIconPathLength;

    /// <summary>full path to the media library being operated on</summary>
    public static string LibraryPath { get; private set; }
    /// <summary>Full path to the folder to save generated icons to</summary>
    public static string IconFolder { get; private set; }

    /// <summary>If true: overwrite files when saving processed icon files</summary>
    private static bool overwrite = false;
    public static bool Overwrite { 
        get => overwrite; 
        set => overwrite = value; 
    }
    /// <summary>If true: crop icons to the dimensions specified in Config.squareIconSize</summary>
    private static bool crop = false;
    public static bool Crop {
        get => crop;
        set => crop = value; 
    }

    /// <summary>character to substiture for seperator chars in source image paths when saving them as icons</summary>
    private static string separatorSubstitute = "$";
    public static string SeparatorSubstitute { 
        get => separatorSubstitute; 
        set => separatorSubstitute = value; 
    }
    /// <summary>character to substiture for non ascii characters in source image paths when saving them as icons</summary>
    private static string nonAsciiSubstitute = "_";
    public static string NonAsciiSubstitute { 
        get => nonAsciiSubstitute; 
        set => nonAsciiSubstitute = value; 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="libraryPath">Full path to the media library to be operated on</param>
    /// <param name="icoFolder">Full path to the folder to save generated icons to</param>
    public static void Init(string libraryPath, string icoFolder) {
            LibraryPath = libraryPath;
            IconFolder = icoFolder;
            maxIconPathLength = maxFilePathLength - (IconFolder.Length + 1 + ext.Length);
    }
    /// <summary>
    /// Initialize config using program arguments. 
    /// args[0] should be a full path to the media file to operate on. 
    /// args[1] should be a full path to the folder to save generated icons to
    /// </summary>
    /// <param name="args">program arguments</param>
    public static void Init(string[] args) {
        //todo: more sanity checking on arguments
        LibraryPath = args[0];
        IconFolder = args[1];
        Overwrite = args[2] == "true" | args[2] == "True" ? true : false;
        Directory.CreateDirectory(IconFolder);
        maxIconPathLength = maxFilePathLength - (IconFolder.Length + 1 + ext.Length);
    }
    /// <summary>
    /// Get a path to save an icon to.
    /// </summary>
    /// <param name="sourceMediaPath">the full path to the source media file which the icon was generated from</param>
    /// <returns>the full path which should be used when saving this icon</returns>
    public static string ToSaveDest(string sourceMediaPath) {
        string parent = Path.GetDirectoryName(sourceMediaPath);
        string iconRP = 
            Path.GetRelativePath(LibraryPath, parent)
            .Replace("\\", SeparatorSubstitute)
            .ReplaceNonAscii(NonAsciiSubstitute);

        if (iconRP.Length > maxIconPathLength) 
            iconRP = iconRP.Remove(maxIconPathLength);

        return Path.Combine(IconFolder, iconRP + ext);
    }
}