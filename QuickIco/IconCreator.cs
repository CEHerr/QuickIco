using static PrimitiveExtensions.StringExt;
using ImageMagick;
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
    public static bool QueueWork(Path path, out Path saveDestination) {
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

    private static class ImageIconCreator {
        static Stack<Path> workQueue = new Stack<Path>();
        //static System.Collections.Concurrent.ConcurrentStack<Path> _workQueue = new();
        public static void QueueWork(Path path) {
            workQueue.Push(path);
            //Parallel.ForEachAsync(_workQueue, foo); //###
            //var foo = Foo;                          //###
            //void Foo(Path inputs) { }
        }
        public static void ProcessQueue() {
            Console.WriteLine("BEGGINING OF WORK");
            foreach (Path imagePath in workQueue) {
                Console.WriteLine($"\t{imagePath}");
                //Path dest = imagePath.GetSaveDest();
                Path dest = ToSaveDest(imagePath);

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
        public static Path ToSaveDest(Path sourceMediaPath) {
            if (!File.Exists(sourceMediaPath))
                throw new FileNotFoundException($"No file exists at {sourceMediaPath}");

            string parent = Directory.GetParent(sourceMediaPath).FullName;

            string iconRelativePath = Path.GetRelativePath(parent)
                .Replace("\\", Config.SeparatorSubstitute)
                .ReplaceNonAscii(Config.NonAsciiSubstitute);

            bool pathTooLong = (iconRelativePath.Length + Config.IcoFolder.path.Length + 1)
                             > (Config.maxFilePathLength - 4);
            if (pathTooLong)
                iconRelativePath = iconRelativePath
                    .Remove(Config.maxFilePathLength - (Config.IcoFolder.path.Length + 1) - 4);

            return new Path(Config.IcoFolder, iconRelativePath + ".ico");

            //Note on unique key: two files will not have the same creation time
        }
    }

}