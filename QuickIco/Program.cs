//call this program with the full path to a media library as the first argument
//the full path to a folder to save generated icons to as the second argument
//and true or false to set overwrite policy
//eg: QuickIco "C:\path\to\library" "C:\path\to\icon\folder" true
class QuickIco {
    const string instructions = "QuickIco takes 3 arguments in the following order:\n 1\tthe full path to a library folder\n 2\tthe full path to the folder to save created icons to\n 3\ttrue to overwrite on naming conflicts when saving icons, false to skip on naming conflicts";
    const string example = ".\\QuickIco \"C:\\path\\to\\library\" \"C:\\path\\to\\icon\\folder\" true";
    static void Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (args[0] == "help") {
            Console.WriteLine(instructions);
            Console.WriteLine("an example of typical usage:\n" + example);
            return;
        }
        if (!Directory.Exists(args[0])) throw new Exception("The specified library path does not exist.\n" + instructions);
#if (timing)
        System.Diagnostics.Stopwatch timer = new();
        timer.Start();
#endif

        //<initialization>
        Config.Init(args);
        Config.Overwrite = true;
        Config.Crop = true;
        
        try { ImageMagick.MagickNET.Initialize(); }
        catch { throw new Exception("ImageMagick failed to initialize"); }

        Folder lib = FolderFactory.CreateFolder(Config.LibraryPath);
        lib.CreateSubFolders(true);
        //</initialization>

        Console.WriteLine("creating Icons...");
        lib.CreateIcon(true);
        Console.WriteLine("Setting Icons...");
        lib.SetIcons(true);

        Console.WriteLine("Completed");
        
#if (timing)
        timer.Stop();
        TimeSpan timeSpan = timer.Elapsed;
        Console.WriteLine("Process took: " + timeSpan.ToString(@"m\:ss\.fff"));
#endif
    }
}