using System.Text;
using static FolderFactory;

class QuickIco {
    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        #if (timing)
        System.Diagnostics.Stopwatch timer = new();
        timer.Start();
        #endif
        //<initialization>
        Config.Init(args);

        try { ImageMagick.MagickNET.Initialize(); }
        catch { throw new Exception("ImageMagick failed to initialize"); }

        Folder lib = CreateFolder(Config.LibraryPath);
        lib.CreateSubFolders(true);
        //</initialization>


        #if (timing)
        timer.Stop();
        TimeSpan timeSpan = timer.Elapsed;
        Console.WriteLine("Process took: " + timeSpan.ToString(@"m\:ss\.fff"));
        #endif
    }
}