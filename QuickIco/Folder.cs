using static SysExtension.Collections;
public static class FolderFactory {
    public static Folder CreateFolder(string path) {
        return new Folder(path);
    }
}
public partial class Folder {
    /// <summary>Full path to this folder</summary>
    public string Path { get; }
    /// <summary>Path with all characters before and including the last seperator char removed</summary>
    public string Name { get => System.IO.Path.GetDirectoryName(this); }
    /// <summary>Object for managing this folder's desktop.ini file</summary>
    private Desktop desktop;
    /// <summary>A list of all subFolders of this folder. Stored as full paths. Call Folder.CreateSubFolders(bool recursive) to populate this list.</summary>
    public List<Folder> subFolders = new List<Folder>();
    /// <summary>
    /// The full file path to the media contained directly inside this folder to be used to generate this folder's icon.
    /// Even if this value is null this folder may still recieve an icon by inheriting one from it's children. See Folder.GetIconPath()
    /// </summary>
    private string? pathToIcon = null;

    public Folder(string path) {
        this.Path = path;
        desktop = new Desktop(this);
    }
    /// <summary>
    /// Instantiate Folder objects for all of this folders subfolders
    /// </summary>
    /// <param name="recursive">If true run this operation recursively down the entire file tree</param>
    public void CreateSubFolders(bool recursive) {
        Each(Directory.GetDirectories(this),
            (sF) => subFolders.Add(new Folder(sF)));
        if (recursive)
            Each(subFolders, (sF) => sF.CreateSubFolders(true));
    }
    /// <summary>
    /// Print the entire file tree bellow this point
    /// </summary>
    public void PrintSubFolders() {
        SysExtension.Tree.PrintTree(
            this,
            (f) => f.subFolders,
            (f) => f.Name);
    }
    /// <summary>
    /// Run Icon creation process for this folder.
    /// Note: this is relient on the work queue having been populated, see Folder.QueueAll(bool recursive)
    /// Note: this does not set the icons after creating them, see Folder.SetIcons(bool recursive)
    /// </summary>
    /// <param name="recursive">If true run this operation recursively down the entire file tree</param>
    public void CreateIcon(bool recursive) {
        QueueAll(recursive);
        IconCreator.ProcessQueue();
    }
    /// <summary>
    /// Determines which media file to use as the source for this folders icon and and queues it for processing. See Folder.CreateIcon(bool recursive) to process.
    /// </summary>
    /// <param name="recursive">If true run this operation recursively down the entire file tree</param>
    public void QueueAll(bool recursive) {
        if (recursive && subFolders.Any())
            Each(subFolders, (sF) => sF.QueueAll(true));
        Queue(GetMediaToQueue());

        void Queue(string? icoSource) {
            if (icoSource is not null && Path != Config.LibraryPath)
                IconCreator.QueueWork(icoSource, out pathToIcon);
        }
    }
    /// <summary>
    /// Performs the system calls necessary to set this folders icon
    /// </summary>
    /// <param name="recursive">If true run this operation recursively down the entire file tree</param>
    public void SetIcons(bool recursive) {
        if (recursive && subFolders.Any())
            Each(subFolders, (f) => f.SetIcons(true));
        if (Path != Config.LibraryPath)
            desktop.SetIcon(GetIconPath());
    }
    /// <returns>The path to the media file which should be used as the source to generate this folders icon.
    /// If this folder has no valid media files it will attempt to inherit one from it's children</returns>
    private string? GetIconPath() {
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
    /// <returns>If this folder contains media: A path to the media file to queue for processing. else: null</returns>
    private string? GetMediaToQueue() {
        //this works by creating a precedence mask based on the file names of contained media files and then
        //selecting the file with the lowest precedence
        IEnumerable<string>? mediaFiles = GetMediaFiles();
        if (mediaFiles is null) return null;

        IEnumerable<string> namesOnly = mediaFiles.Select((x) =>
            System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(x), null));

        int index = GetIndex(namesOnly.Select(ToPrecedence));

        return mediaFiles.ElementAt(index);

        //todo: move this into a .config file
        int ToPrecedence(string path) => path switch {
            "icon"          => 0,
            "ico"           => 1,
            "cover"         => 2,
            "cover art"     => 3,
            "album cover"   => 4,
            "book cover"    => 5,
            "folder"        => 6,

            _ => 999,
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
    /// <returns>Full file paths of all media files in this folder</returns>
    private IEnumerable<string>? GetMediaFiles() {
        var mediaFiles = Directory.GetFiles(Path).Where(IOext.Path.IsMedia);

        return mediaFiles.Any() ? mediaFiles : null;
    }


    public override string ToString() { return Path; }
    public static implicit operator string(Folder f) => f.Path;

#if (experimentalFeatures)
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
}