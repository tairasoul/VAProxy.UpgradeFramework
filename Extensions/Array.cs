using System;

namespace UpgradeFramework.Extensions
{
    public static class ArrayExtensions
    {
        public static T Find<T>(this T[] arr, Func<T, bool> predicate)
        {
            foreach (T item in arr)
            {
                if (predicate(item)) return item;
            }
            return default;
        }
    }
}
