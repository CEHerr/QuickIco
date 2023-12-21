using ImageMagick;
using static System.IO.Path;
//split this class into two classes with a factory
//one for folders
//one for files
public class Path {
    public string path {  get; private set; }
    public string? Extension   {
        get { 
            int length = path.Length;
            for (int i = length; --i >= 0;) {
                char c = path[i];
                if (c == '.') 
                    return path.Substring(i, length - i);
                if (c == '\\' | c == '/')
                    break;
            }
            return null;
        } 
    }
    public string name { 
        get => GetFileName(path);
    }
    public string ExtlessName { 
        get { 
            return ChangeExtension(name, null); 
        }
    }

#region constructors
    public Path(string path) {
        this.path = path;
    }
    //create a Path by appending the provided string to another path
    //note that instances of Path and Folder may be implicitly casted to strings
    public Path(string path, string name) {
        this.path = Combine(path, name);
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
        string relParentPath = GetRelativePath(parentInfo.FullName);
        return IcoFromRel(relParentPath);
    }
    //returns the path to a folders icon ; use GetRelativePath to create this methods arg
    private Path IcoFromRel(string relativePath) {
        string icoName = relativePath.Replace("\\", "$");

        //new naming logic goes here


        int icoFolderPathLength = Config.IcoFolder.path.Length + 1;
        bool pathTooLong = (icoName.Length + icoFolderPathLength) > (Config.maxFilePathLength - 4);

        if (pathTooLong) {
            icoName = icoName.Remove(Config.maxFilePathLength - icoFolderPathLength - 4);
        }

        return new Path(Config.IcoFolder, icoName + ".ico");
    }

    //returns the path with the library's path removed
    public string GetRelativePath() {
        return GetInfo().FullName.Substring(Config.LibraryPath.path.Length + 1);
    }
    public static string GetRelativePath(string _path) {
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