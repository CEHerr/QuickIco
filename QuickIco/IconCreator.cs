using static PrimitiveExtensions.StringExt;
using ImageMagick;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
//Factor out the entire framework of this into a generic WorkQueue<T> Class?
public static class IconCreator {
    //this is used to garuntee a unique path to all icons
    private static int Id = 0;
    /// <summary>
    /// Adds an image to the work queue of the approprate Icon Creator. Process these queues by calling ProcessQueue()
    /// </summary>
    /// <param name="path">The path to the media file to be used to generate the icon</param>
    /// <param name="saveDestination">The path which will be saved to by the icon creator when the media is processed or null if the file failed to be queued</param>
    /// <returns>True if the media has been successfuly added to a work queue, else false.</returns>
    public static bool QueueWork(__DEPRICATED_PATH__ path, out __DEPRICATED_PATH__ saveDestination) {
        switch (path.Extension) {
            case ".jpg":
                saveDestination = ImageIconCreator.ToSaveDest(path);
                ImageIconCreator.QueueWork(path);
                return true;
            case ".jpeg":
                saveDestination = ImageIconCreator.ToSaveDest(path);
                ImageIconCreator.QueueWork(path);
                return true;
            case ".png":
                saveDestination = ImageIconCreator.ToSaveDest(path);
                ImageIconCreator.QueueWork(path);
                return true;
            default:
                saveDestination = null;
                return false;
        }
    }
    public static void ProcessQueue() {
        ImageIconCreator.ProcessQueue();
    }

    public static class ImageIconCreator {
        static Stack<__DEPRICATED_PATH__> workQueue = new Stack<__DEPRICATED_PATH__>();
        //static System.Collections.Concurrent.ConcurrentStack<__DEPRICATED_PATH__> _workQueue = new();
        public static void QueueWork(__DEPRICATED_PATH__ path) {
            workQueue.Push(path);
            //Parallel.ForEachAsync(_workQueue, foo); //###
            //var foo = Foo;                          //###
            //void Foo(__DEPRICATED_PATH__ inputs) { }
        }
        public static void ProcessQueue() {
            Console.WriteLine("BEGGINING OF WORK");
            foreach (__DEPRICATED_PATH__ imagePath in workQueue) {
                Console.WriteLine($"\t{imagePath}");
                //__DEPRICATED_PATH__ dest = imagePath.GetSaveDest();
                __DEPRICATED_PATH__ dest = ToSaveDest(imagePath);

                if (!Config.Overwrite && dest.Exists())
                    continue;

                MagickImage img = new();
                try { 
                    img.Read(imagePath); }
                catch { 
                    Console.WriteLine($"ImageMagick failed to read the image at {imagePath}");
                    continue;
                }

                if (Config.Crop) {
                    int sqSize = Math.Min(img.Width, img.Height);
                    try { 
                        img.Crop(sqSize, sqSize); }
                    catch { 
                        Console.WriteLine($"ImageMagick failed to crop the image at {imagePath}");
                        continue;
                    }
                }
                try { 
                    img.Resize(new MagickGeometry(Config.squareIcoSize)); }
                catch {
                    Console.WriteLine($"ImageMagick failed to resize the image at {imagePath}");
                    continue;
                }

                try { 
                    img.Write(dest); }
                catch {
                    Console.WriteLine($"ImageMagick failed to save the processed image from {imagePath}\nto the save destination {dest}");
                    continue;
                }
#if verbose
                Console.WriteLine($"Icon created for {imagePath}");
#endif
            }
        }
        /// <summary>
        /// Creates the file path which will be saved too when the image at the specified path is processed.
        /// </summary>
        /// <param name="sourceMediaPath">path to the image which will be processed</param>
        /// <returns>save destination path</returns>
        public static __DEPRICATED_PATH__ ToSaveDest(__DEPRICATED_PATH__ sourceMediaPath) {
            Contract.Requires(File.Exists(sourceMediaPath));
            if (!File.Exists(sourceMediaPath))
                throw new FileNotFoundException($"No file exists at {sourceMediaPath}");

            string parent = Directory.GetParent(sourceMediaPath).FullName;

            string iconRelativePath = __DEPRICATED_PATH__.GetRelativePath(parent)
                .Replace("\\", Config.SeparatorSubstitute)
                .ReplaceNonAscii(Config.NonAsciiSubstitute);

            bool pathTooLong = (iconRelativePath.Length + Config.IcoFolder.path.Length + 1)
                             > (Config.maxFilePathLength - 4);
            if (pathTooLong)
                iconRelativePath = iconRelativePath
                    .Remove(Config.maxFilePathLength - (Config.IcoFolder.path.Length + 1) - 4);

            return new __DEPRICATED_PATH__(Config.IcoFolder, iconRelativePath + ".ico");

            //Note on unique key: two files will not have the same creation time
        }
    }

}