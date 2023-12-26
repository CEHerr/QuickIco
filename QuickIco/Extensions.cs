namespace PrimitiveExtensions {
    public static class StringExt {
        /// <summary>
        /// Replace all Non-Ascii characters in this string with another character
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacement">The replacement character</param>
        /// <returns></returns>
        public static string ReplaceNonAscii(this string str, string replacement) {
            const string nonAsciiRegexPattern = @"[^\x00-\x7F]";
            return System.Text.RegularExpressions.
                Regex.Replace(str, nonAsciiRegexPattern, replacement);
        }
    }
}


namespace CollectionExtensions {
    public static class IEnumerableExt {
        /// <summary>Shorthand for a simple foreach loop</summary>
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action) {
            foreach (var item in collection) action(item);
        }
        public static string AsString(this IEnumerable<char> e) {
            return string.Concat(e);
        }
    }
}


namespace IOext {
    public enum FileType {
        Image,
        Audio,
        Video,

        Unrecognized,
    }
    public static partial class Path {
        public static FileType ToFileType(string path) => 
            System.IO.Path.GetExtension(path) switch {
            ".jpg" => FileType.Image,
            ".jpeg"=> FileType.Image,
            ".png" => FileType.Image,

            _      => FileType.Unrecognized,
        };
        public static bool IsMedia(string path) => ToFileType(path) switch {
            FileType.Image => true,
            FileType.Audio => true,
            FileType.Video => true,

            _ => false,
        };
    }
}