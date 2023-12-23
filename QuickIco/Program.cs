using System.Text;
using static FolderContextFactory;
using static FolderOptionsFactory;
using static FolderFactory;

class QuickIco {
    static void Main(string[] args) {

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
        string const myPath = @"C:\BACKUP\Ero\doujin";
        var koo = System.IO.Path.GetDirectoryNameOffset();
        var test = System.IO.Path.GetRelativePath(
            @"C:\BACKUP\Ero\doujin",
            @"C:\BACKUP\Ero\doujin\Accio\[Accio] Minato Matome [English]");
        Console.WriteLine(test);
        //system call method of setting icons is working
        //Folder testFold = new(@"C:\BACKUP\Ero\doujin\Accio");
        //__DEPRICATED_PATH__ testPath = new(@"C:\BACKUP\Software\!mine\icoSys\icons\Aimaitei Umami$[Aimaitei (Aimaitei Umami)] Futanari Alter-tachi ni Josou Shita Ore ga Okasareru Hanashi (Fate Grand Order) [English] {Hennojin} [Digital].ico");
        //testFold.desktop.SetIcon(testPath);
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