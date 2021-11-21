namespace System.Collections.Generic
{
    public static class SpriteListExtensions
    {
        public static List<T> SortByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            /* From:  https://www.codeproject.com/script/Membership/View.aspx?mid=571194    
             * Articles by: Tomas Takac Topological-sorting-in-Csharp
             */

            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }

        public static void MoveItem<T>(this List<T> source, Predicate<T> selector, int targetIndex)
        {
            if (!targetIndex.IsBetween(0, source.Count - 1))
            {
                throw new IndexOutOfRangeException("targetIndex should be between 0 and " + (source.Count - 1));
            }


            var currentIndex = source.FindIndex(0, selector);
            if (currentIndex == targetIndex)
            {
                return;
            }

            var item = source[currentIndex];
            source.RemoveAt(currentIndex);
            source.Insert(targetIndex, item);
        }
    }
}