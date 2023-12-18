using ImageMagick;
using static System.IO.Path;
//split this class into two classes with a factory
//one for folders
//one for files
public class Path {
    private FileInfo info;
    public string path        { get { return info.FullName; } }
    public string Extension   { get { return info.Extension; } }
    public string name        { get { return info.Name; } }
    public string ExtlessName { get { return ChangeExtension(name, null); }
    }

#region constructors
    public Path(string path) {
        //###this throws if the file specified by path does not exist
        info = new FileInfo(path);
    }
    //create a Path by appending the provided string to another path
    //note that instances of Path and Folder may be implicitly casted to strings
    public Path(string path, string name) {
        info = new FileInfo(Combine(path, name));
    }
#endregion

    //### this is only an error check before calling ParentToIco(), possibly move error check to IconCreator and call directly
    //returns icon path to be used for this files parent folder
    public Path GetSaveDest() {
        if (!IsFile()) { throw new Exception("GetSaveDest() called on a directory"); }
        
        return ParentToIco();
    }
    
    //returns icon path to be used for this folder
    public Path ToIco() {
        string relativePath = GetRelativePath();
        return IcoFromRel(relativePath);
    }
    //returns icon path to be used for this folder/file's parent
    private Path ParentToIco() {
        DirectoryInfo? parentInfo = Directory.GetParent(path);
        if (parentInfo is null) { throw new Exception($"the path {path} has no parent"); }
        string parentPath = parentInfo.FullName;
        string relParentPath = GetRelativePath(parentPath);
        return IcoFromRel(relParentPath);
    }
    //returns the path to a folders icon ; use GetRelativePath to create this methods arg
    private Path IcoFromRel(string relativePath) {
        string icoName = relativePath.Replace("\\", "$");

        int icoFolderPathLength = Config.IcoFolder.path.Length + 1;
        bool pathTooLong = (icoName.Length + icoFolderPathLength) > (Config.maxFilePathLength - 4);

        if (pathTooLong) {
            icoName = icoName.Remove(Config.maxFilePathLength - icoFolderPathLength - 4);
        }

        return new Path(Config.IcoFolder, icoName + ".ico");
    }

    //returns the path with the library's path removed
    private string GetRelativePath() {
        return GetInfo().FullName.Substring(Config.LibraryPath.path.Length + 1);
    }
    private string GetRelativePath(string _path) {
        DirectoryInfo inf = new(_path);
        return inf.FullName.Substring(Config.LibraryPath.path.Length + 1);
    }
    //returns directoryinfo for this folder/file
    private DirectoryInfo GetInfo() {
        return new DirectoryInfo(path);
    }
    private bool IsFile() {
        return File.Exists(path);
    }

    public bool Exists() {
        return File.Exists(this);
    }

    public static implicit operator string(Path p) => p.path;

    public override string ToString() { return path; }
}