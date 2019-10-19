using System.Collections.Generic;

namespace TicTacToe.Core {
    public static class Util {
        public static string CollectionToString<T>(this IEnumerable<T> values) {
            return $"[{string.Join(", ", values)}]";
        }
    }
}