namespace SysExtension {
    public static class Collections {
        /// <summary>
        /// Works like linq Select() but with actions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void Each<T>(IEnumerable<T> collection, Action<T> action) {
            foreach (var item in collection) action(item);
        }
        public static void EachIf<T>(bool condition, IEnumerable<T> collection, Action<T> action) {
            if (condition) {
                foreach(var item in collection) action(item);
            }
        }
    }
}
