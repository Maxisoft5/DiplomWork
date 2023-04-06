using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm.Extensions
{
    public static class LinqExtensions
    {
        public static int IndexOfMin<T>(this IEnumerable<T> array)
            where T : IComparable
        {
            int index = 0;
            for (int i = 1; i < array.Count(); i++)
            {
                if (array.ElementAt(i).CompareTo(array.ElementAt(index)) < 0)
                {
                    index = i;
                }
            }
            return index;
        }

        public static int IndexOfMax<T>(this IEnumerable<T> array)
            where T : IComparable
        {
            int index = 0;
            for (int i = 1; i < array.Count(); i++)
            {
                if (array.ElementAt(i).CompareTo(array.ElementAt(index)) > 0)
                {
                    index = i;
                }
            }
            return index;
        }
    }
}
