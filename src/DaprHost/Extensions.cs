namespace Applinate
{
    internal static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> f)
        {
            foreach(var item in items)
            {
                f(item);
            }
        }
    }
}