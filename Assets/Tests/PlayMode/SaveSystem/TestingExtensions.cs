using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace SaveSystemTests
{
    public static class TestingExtensions
    {
        public static IList<string> GetValues(this IList<StringPair> pairs)
        {
            IList<string> result = new string[pairs.Count];

            for (int i = 0; i < pairs.Count; i++)
            {
                var currentPair = pairs[i];
                var currentVal = currentPair.val;
                result[i] = currentVal;
            }

            return result;
        }

        public static bool HasSameContentsInOrderAs<T>(this IList<T> firstList, IList<T> secondList)
        {
            bool differentSizes = firstList.Count != secondList.Count;

            if (differentSizes)
                return false;

            for (int i = 0; i < firstList.Count; i++)
            {
                var firstItem = firstList[i];
                var secondItem = secondList[i];

                if (!firstItem.Equals(secondItem))
                    return false;
            }

            return true;
        }
    }
}