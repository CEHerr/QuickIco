﻿using ImageMagick;
public static class IconCreator {
    /// <summary>
    /// Adds an image to the work queue of the approprate Icon Creator. Process these queues by calling ProcessQueue()
    /// </summary>
    /// <param name="path">The path to the media file to be used to generate the icon</param>
    /// <param name="saveDestination">The path which will be saved to by the icon creator when the media is processed or null if the file failed to be queued</param>
    /// <returns>True if the media has been successfuly added to a work queue, else false.</returns>
    public static bool QueueWork(string path, out string saveDestination) {
        switch (IOext.Path.ToFileType(Path.GetExtension(path))) {
            case IOext.FileType.Image:
                saveDestination = Config.ToSaveDest(path);
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
        static Stack<string> workQueue = new();
        public static void QueueWork(string path) {
            workQueue.Push(path);
        }
        public static void ProcessQueue() {
            foreach (string imagePath in workQueue) {
                string dest = Config.ToSaveDest(imagePath);

                if (!Config.Overwrite && Path.Exists(dest))
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
    }
}