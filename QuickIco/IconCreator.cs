using ImageMagick;

public static class IconCreator {
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
        public static void QueueWork(Path path) {
            workQueue.Push(path);
        }
        public static void ProcessQueue() {
            foreach (Path imagePath in workQueue) {
                Path dest = imagePath.GetSaveDest();
                if (!Config.Overwrite && dest.Exists()) { continue; }

                MagickImage img = new();
                img.Read(imagePath);

                if (Config.Crop) {
                    int sqSize = Math.Min(img.Width, img.Height);
                    img.Crop(sqSize, sqSize);
                }
                img.Resize(new MagickGeometry(Config.squareIcoSize));

                img.Write(dest);
#if verbose
                Console.WriteLine($"Icon created for {imagePath}");
#endif
            }
        }
        public static Path ToSaveDest(Path sourceMediaPath) {
            return sourceMediaPath.GetSaveDest();
        }
    }

}