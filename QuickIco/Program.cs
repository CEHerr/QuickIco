using System.Text;
using static FolderContextFactory;
using static FolderOptionsFactory;
using static FolderFactory;

class QuickIco {
    static void Main(String[] args) {

#if (timing)
        System.Diagnostics.Stopwatch timer = new();
        timer.Start();
#endif

#region initialization
        string libPath = args[0];
        Config.Init(true, libPath);
        Console.OutputEncoding = Encoding.UTF8;

        try {
            ImageMagick.MagickNET.Initialize();
        }
        catch { throw new Exception("ImageMagick failed to initialize"); }

        Folder lib = CreateFolder(Config.LibraryPath);
        lib.CreateSubFolders(true);
#endregion

        Folder testFold = new(@"C:\BACKUP\Ero\doujin\Aimaitei Umami");
        testFold.CreateSubFolders(true);
        testFold.QueueAll(true);
        Console.WriteLine($"icon to use for folder {testFold}:\n" + testFold.GetIconPath());

#if (create)
        lib.CreateSubFolders(true);
        lib.CreateIcon(true);
        lib.SetIcons(true);
#endif

#if (timing)
        timer.Stop();
        TimeSpan timeSpan = timer.Elapsed;
        Console.WriteLine("Process took: " + timeSpan.ToString(@"m\:ss\.fff"));
#endif
    }
}