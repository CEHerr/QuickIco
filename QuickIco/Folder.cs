using static SysExtension.Collections;
public static class FolderFactory {
    public static Folder CreateFolder(__DEPRICATED_PATH__ path) {
        return new Folder(path);
    }
}
public partial class Folder { 
    public __DEPRICATED_PATH__ path { get; }
    public Desktop desktop;
    public List<Folder> subFolders = new List<Folder>();
    public bool hasIcon = false;
    private __DEPRICATED_PATH__? pathToIcon = null;

    public Folder(__DEPRICATED_PATH__ path) {
        this.path = path;
        desktop = new Desktop(this);
    }
    public Folder(string path) {
        this.path = new __DEPRICATED_PATH__(path);
        desktop = new Desktop(this);
    }
    public Folder(string path, Folder parent) {
        this.path = new __DEPRICATED_PATH__(path);
        desktop = new Desktop(this);
        #if (verbose)
        Console.WriteLine($"folder created for {path}");
        #endif
    }
    public void CreateSubFolders(bool recursive) {
        Each(Directory.GetDirectories(this),
            (sF) => subFolders.Add(new Folder(sF, this)));
        if (recursive) 
            Each(subFolders, (sF) => sF.CreateSubFolders(true));
    }
    public void PrintSubFolders() {
        SysExtension.Tree.PrintTree(
            this, 
            (f) => f.subFolders, 
            (f) => f.Name());
    }

    public void CreateIcon(bool recursive) {
        QueueAll(recursive);
        IconCreator.ProcessQueue();
    }
    public void QueueAll(bool recursive) {
        if (recursive && subFolders.Any()) 
            Each(subFolders, (sF) => sF.QueueAll(true));
        Queue(GetIconSourceImage());

        void Queue(__DEPRICATED_PATH__? icoSource) {
            if (icoSource is not null && path != Config.LibraryPath)
                IconCreator.QueueWork(icoSource, out pathToIcon);
        }
    }

    public void SetIcons(bool recursive) {
        if (recursive && subFolders.Any())
            Each(subFolders, (f) => f.SetIcons(true));
        if (path != Config.LibraryPath)
            desktop.SetIcon(GetIconPath());
    }
    public string? GetIconPath() {
        if (pathToIcon is not null)  
            return pathToIcon;
        return AttemptInheritence();

        string? AttemptInheritence() {
            foreach (Folder sub in subFolders) {
                string res = sub.GetIconPath();
                if (res is null) { continue; }
                else { return res; }
            }
            return null;
        }
    }
    /// <summary>
    /// Get's the filepath of the image to use to generate the icon for this folder
    /// </summary>
    /// <returns>If this folder contains media: A __DEPRICATED_PATH__ object for it's Icon Source Image. else: null</returns>
    public __DEPRICATED_PATH__? GetIconSourceImage() {
        IEnumerable<__DEPRICATED_PATH__>? mediaFiles = GetMediaFiles();
        if (mediaFiles is null) return null;

        IEnumerable<string> namesOnly = mediaFiles.Select((x) => x.ExtlessName);
        int index = GetIndex(namesOnly.Select(ToPrecedence));
        return new __DEPRICATED_PATH__(mediaFiles.ElementAt(index));

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
    /// Change this to return IEnumerable of type __DEPRICATED_PATH__
    /// </summary>
    /// <returns>Fully qualified file paths of all media files in this folder</returns>
    private IEnumerable<__DEPRICATED_PATH__>? GetMediaFiles(){
        string[]? files = Directory.GetFiles(path);
        var mediaFiles = from f in files
                          where IsMedia(f)
                          select new __DEPRICATED_PATH__(f);
        
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
    public void _PrintSubFolders(bool recursive) {
        if (subFolders.Any()) {
            Console.WriteLine(Name() + ":");
            //Each(subFolders, (sF)=> sF.PrintSubFolders(recursive));
            foreach (Folder subFolder in subFolders) {
                Console.WriteLine("\t" + subFolder.Name());
                if (recursive) { subFolder.PrintSubFolders(true); }
            }
        }

    }
    private string _GetIconPath() {
        string emptyCase = "";
        if (path == Config.LibraryPath) { return emptyCase; }                   //we are in the library root
        else if (hasIcon)               { return path.ToIco(); }                //the folder contains media
        else if (subFolders.Any())      { return subFolders[0].GetIconPath(); } //the folder has no media, but does have children
        else                            { return emptyCase; }                   //folder has no media or children
    }
#endif
}