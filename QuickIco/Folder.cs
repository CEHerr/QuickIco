using static SysExtension.Collections;
public static class FolderFactory {
    public static Folder CreateFolder(Path path) {
        return new Folder(path);
    }
}
public partial class Folder { 
    public Path path { get; }
    public Desktop desktop;
    public List<Folder> subFolders = new List<Folder>();
    public bool hasIcon = false;
    private Path? pathToIcon = null;
#region constructors
    public Folder(Path path) {
        this.path = path;
        desktop = new Desktop(this);
    }
    public Folder(string path) {
        this.path = new Path(path);
        desktop = new Desktop(this);
    }
    public Folder(string path, Folder parent) {
        this.path = new Path(path);
        desktop = new Desktop(this);
    }
#endregion
    public void CreateSubFolders(bool recursive) {
        Each(Directory.GetDirectories(this),
            (sF) => subFolders.Add(new Folder(sF, this)));
        if (recursive) 
            Each(subFolders, (sF) => sF.CreateSubFolders(true));
        /*
        foreach (string subFolder in Directory.GetDirectories(this)) {
            subFolders.Add(new Folder(subFolder, this));
        }
        if (recursive) {
            foreach (Folder subFolder  in subFolders) {
                subFolder.CreateSubFolders(true);
                //Each(subFolders, (sF) => Console.WriteLine(sF));
            }
        }
        */
    }
    public void PrintSubFolders(bool recursive) {
        if (subFolders.Any()) {
            Console.WriteLine(Name() + ":");
            foreach (Folder subFolder in subFolders) {
                Console.WriteLine("\t" + subFolder.Name());
                if (recursive) { subFolder.PrintSubFolders(true); }
            }
        }
    }
    public void CreateIcon(bool recursive) {
        QueueAll(recursive);
        IconCreator.ProcessQueue();
    }
    public void QueueAll(bool recursive) {
        if (recursive && subFolders.Any()) 
            Each(subFolders, (sF) => sF.QueueAll(true));
        //change Queue to use an out parameter which is writes the ultimate destination to
        Queue(GetIconSourceImage());
        //### refactor
        void Queue(Path? icoSource) {
            if (icoSource is not null && path != Config.LibraryPath) {
                if (IconCreator.QueueWork(icoSource, out pathToIcon)) {
#if verbose
                    Console.WriteLine($"Icon queued for {icoSource}");
#endif
                }
            }
        }
    }

    public void SetIcons(bool recursive) {
        //EachIf(recursive && subFolders.Any(),
        //    subFolders, (sF) => sF.SetIcons(true));
        if (recursive && subFolders.Any()) {
            foreach (var folder in subFolders) {
                folder.SetIcons(true);
            }
        }
        if (path != Config.LibraryPath) {
            if (desktop.SetIcon(GetIconPath())) {
#if verbose
                Console.WriteLine($"Icon set for {path}");
#endif
            }; 
        }
    }

    //### refactor
    private string _GetIconPath() {
        string emptyCase = "";
        if (path == Config.LibraryPath) { return emptyCase; }                   //we are in the library root
        else if (hasIcon)               { return path.ToIco(); }                //the folder contains media
        else if (subFolders.Any())      { return subFolders[0].GetIconPath(); } //the folder has no media, but does have children
        else                            { return emptyCase; }                   //folder has no media or children
    }
    public string? GetIconPath() {
        //this method needs to return the REAL FINAL path to the icon to use
        //this method must impliment inheritence from children down to arbitrary depth and
        //accross all children
        if (pathToIcon is not null) { return pathToIcon; }
        //this folder has no valid icon of it's own, we must attempt to inherit
        return AttemptInheritence();



        string? AttemptInheritence() {
            foreach (Folder sub in subFolders) {
                string res = sub.GetIconPath();
                if (res is null) { continue; }
                else { return res; }
            }
            return null;
        }
        //I think this is working
        //I wrote this while drunk, please check my work
    }


    /// <summary>
    /// Get's the filepath of the image to use to generate the icon for this folder
    /// </summary>
    /// <returns>If this folder contains media: A Path object for it's Icon Source Image. else: null</returns>
    public Path? GetIconSourceImage() {
        var mediaFiles = GetMediaFiles();
        if (mediaFiles is null) return null;
        var namesOnly = from f in mediaFiles
                        select f.ExtlessName;
        var _namesOnly = mediaFiles.Select((x) => x.ExtlessName);
        int index = GetIndex(namesOnly.Select(ToPrecedence));
        return new Path(mediaFiles.ElementAt(index));

        //the input must first be prunned to just the name
        //of the file with no extension or path
        int ToPrecedence(string path) => path switch {
            "icon"          => 0,
            "ico"           => 1,
            "cover"         => 2,
            "cover art"     => 3,
            "album cover"   => 4,
            "book cover"    => 5,
            "folder"        => 6,

            _               => 999,
        };
        int GetIndex(IEnumerable<int> pMask) {
            if (pMask.Count() < 2) { return 0; }

            int result = 0;
            for (int i = 1; i < pMask.Count(); i++) {
                result = pMask.ElementAt(result) <= pMask.ElementAt(i) ? result : i;
            }
            return result;
        }
    }
    /// <summary>
    /// Change this to return IEnumerable of type Path
    /// </summary>
    /// <returns>Fully qualified file paths of all media files in this folder</returns>
    private IEnumerable<Path>? GetMediaFiles(){
        string[]? files = Directory.GetFiles(path);
        var mediaFiles = from f in files
                          where IsMedia(f)
                          select new Path(f);
        
        return mediaFiles.Any() ? mediaFiles : null;

        bool IsMedia(string path) {
            FileInfo inf = new FileInfo(path);
            return CheckExtension(inf.Extension);

            bool CheckExtension(string ext) => ext switch {
                ".jpg" => true,
                ".jpeg"=> true,
                ".png" => true,
                _ => false,
            };
        }
    }


    public string Name() { return path.name; }
    public override string ToString() { return path.path; }
    public static implicit operator string(Folder f) => f.path.path;

#if experimentalFeatures
    public void Backup(bool recursive) {
        //###this throws if the backup folder doesn't exist
        try {
            using (StreamWriter sw = new StreamWriter(Config.BackupPath)) {
                using (StreamReader sr = new StreamReader($"{desktop}")) {
                    string? line;
                    sw.WriteLine($"{path}");
                    while ((line = sr.ReadLine()) is not null) {
                        sw.WriteLine(line);
                    }
                }
            }
        }
        catch {
            Console.WriteLine("backup failed");
        }
    }
#endif
#if depricatedMethods
    private Path? GetIconSourceImage() {
        IEnumerable<string>? mediaFiles = GetMediaFiles();
        if (mediaFiles is not null) {
            //move this into a function that checks for a valid icon in the icon folder directly
            hasIcon = true;
            return new Path(mediaFiles.ElementAt(0));
        }
        else { return null; }

        return mediaFiles is not null ? new Path(mediaFiles.ElementAt(0)) : null;
    }
#endif
}