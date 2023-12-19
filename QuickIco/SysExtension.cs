using System.Runtime.CompilerServices;

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
    public static class Tree {
        //methods for data structures in which there are nodes which may have
        //one or more child nodes and always have exactly one parent node
        /// <summary>
        /// Prints a node and all of it's child nodes recursively until the terminus of every branch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">A node within a tree-like data structure</param>
        /// <param name="childNodes">A delegate that takes a node in the tree as an argument and returns a collection containing it's children</param>
        /// <param name="depth"></param>
        public static void PrintTree<T>(T node, Func<T,IEnumerable<T>> childNodes, int depth = 0) {
            Console.WriteLine(StringMaker.Repeat('\t', depth) + node);
            var children = childNodes(node);
            if (children.Any()) {
                foreach (var child in children) {
                    PrintTree(child, childNodes, depth + 1);
                }
            }
        }
        /// <summary>
        /// Prints a node and all of it's child nodes recursively until the terminus of every branch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">A node within a tree-like data structure</param>
        /// <param name="childNodes">A delegate that takes a node in the tree as an argument and returns a collection containing it's children</param>
        /// <param name="nodeName">A delegate that takes a node in the tree as an argument and returns an identifying string for that node</param>
        /// <param name="depth"></param>
        public static void PrintTree<T>(T node, Func<T, IEnumerable<T>> childNodes,Func<T,string> nodeName, int depth = 0) {
            Console.Write(StringMaker.Repeat('\t', depth) + nodeName(node));
            var children = childNodes(node);
            if (children.Any()) {
                foreach (var child in children) {
                    PrintTree(child, childNodes, nodeName, depth + 1);
                }
            }
        }
    }
    //PrintTree(this, (f) => f.subFolders, (f) => f.name);

    public static class StringMaker {
        public static string Repeat(char c, int n) {
            string r = string.Empty;
            for (int i = 0; i < n; i++) {
                r = string.Concat(r, c);
            }
            return r;
        }
    }
}
