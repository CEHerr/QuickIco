using static SysExtension.Collections;
using System.IO;
public static class FolderFactory {
    public static Folder CreateFolder(string path) {
        return new Folder(path);
    }
}
public partial class Folder { 
    public string Path { get; }
    public string Name { get => System.IO.Path.GetDirectoryName(this); }
    public Desktop desktop;
    public List<Folder> subFolders = new List<Folder>();
    public bool hasIcon = false;
    private string? pathToIcon = null;

    public Folder(string path) {
        this.Path = path;
        desktop = new Desktop(this);
    }
    public void CreateSubFolders(bool recursive) {
        Each(Directory.GetDirectories(this),
            (sF) => subFolders.Add(new Folder(sF)));
        if (recursive) 
            Each(subFolders, (sF) => sF.CreateSubFolders(true));
    }
    public void PrintSubFolders() {
        SysExtension.Tree.PrintTree(
            this, 
            (f) => f.subFolders, 
            (f) => f.Name);
    }

    public void CreateIcon(bool recursive) {
        QueueAll(recursive);
        IconCreator.ProcessQueue();
    }
    public void QueueAll(bool recursive) {
        if (recursive && subFolders.Any()) 
            Each(subFolders, (sF) => sF.QueueAll(true));
        Queue(GetIconSourceImage());

        void Queue(string? icoSource) {
            if (icoSource is not null && Path != Config.LibraryPath)
                IconCreator.QueueWork(icoSource, out pathToIcon);
        }
    }

    public void SetIcons(bool recursive) {
        if (recursive && subFolders.Any())
            Each(subFolders, (f) => f.SetIcons(true));
        if (Path != Config.LibraryPath)
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
    /// <returns>If this folder contains media: A path to it's Icon Source Image. else: null</returns>
    public string? GetIconSourceImage() {
        IEnumerable<string>? mediaFiles = GetMediaFiles();
        if (mediaFiles is null) return null;

        IEnumerable<string> namesOnly = mediaFiles.Select((x) => 
            System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(x), null));

        int index = GetIndex(namesOnly.Select(ToPrecedence));

        return mediaFiles.ElementAt(index);

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
    /// 
    /// </summary>
    /// <returns>Fully qualified file paths of all media files in this folder</returns>
    private IEnumerable<string>? GetMediaFiles(){
        var mediaFiles = Directory.GetFiles(Path).Where(IsMedia);
        
        return mediaFiles.Any() ? mediaFiles : null;

        bool IsMedia(string path) => System.IO.Path.GetExtension(path) switch {
            ".jpg" => true,
            ".jpeg" => true,
            ".png" => true,
            _ => false,
        };
    }


    public override string ToString() { return Path; }
    public static implicit operator string(Folder f) => f.Path;

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