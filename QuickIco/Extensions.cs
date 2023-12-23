namespace PrimitiveExtensions {
    public static class StringExt {
        public static string ReplaceNonAscii(this string str, string replacement) {
            const string nonAsciiRegexPattern = @"[^\x00-\x7F]";
            return System.Text.RegularExpressions.
                Regex.Replace(str, nonAsciiRegexPattern, replacement);
        }
    }
}
namespace CollectionExtensions {
    public static class IEnumerableExt {
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action) {
            foreach (var item in collection) action(item);
        }
        public static string AsString(this IEnumerable<char> e) {
            return string.Concat(e);
        }
    }
}
