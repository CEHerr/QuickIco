//discovers, stores and provides access to
//information about the relationship of the
//specified path to other folders
public static class FolderContextFactory {
    public static FolderContext CreateContext(__DEPRICATED_PATH__ path) {
        return new FolderContext(path);
    }
}
public class FolderContext {
    public bool isRoot {  get; }
    public bool isProtected { get; }
    public bool hasSubs { get; }

    public FolderContext(__DEPRICATED_PATH__ path) {
        isRoot = false;
        isProtected = false;
        hasSubs = true;
    }
    public FolderContext(string path) {
        isRoot = false;
        isProtected = false;
        hasSubs = true;
    }

    public override string ToString() {
        return $"this path:\nis a root dir:\t{isRoot}\nis protected:\t{isProtected}\nhas subdirs:\t{hasSubs}";
    }
}