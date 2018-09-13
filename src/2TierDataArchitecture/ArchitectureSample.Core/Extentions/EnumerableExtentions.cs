using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchitectureSample.Core
{
    public static partial class EnumerableExtensions
    {
        public static string ToJoinedString<T>(this IEnumerable<T> source, string separator = "")
        {
            return string.Join(separator, source);
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] values)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.ConcatCore(values);
        }

        private static IEnumerable<TSource> ConcatCore<TSource>(this IEnumerable<TSource> source, params TSource[] values)
        {
            foreach (TSource item in source)
                yield return item;
            foreach (TSource x in values)
                yield return x;
        }

        /// <summary>
        /// <pre>Get random for desired sample count. Same value may output "multiple times"</pre>
        /// <pre>Use Shuffle instead, if you want to avoid "same valut output"</pre>
        /// <pre>Use ShuffleOnce if you want only 1 value.</pre>
        /// </summary>
        public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (sampleCount <= 0)
                throw new ArgumentOutOfRangeException("sampleCount");

            return SampleCore(source, sampleCount, RandomUtil.ThreadRandom);
        }

        /// <summary>
        /// <pre>Get random for desired sample count. Same value may output "multiple times"</pre>
        /// <pre>Use Shuffle instead, if you want to avoid "same valut output"</pre>
        /// <pre>Use ShuffleOnce if you want only 1 value.</pre>
        /// </summary>
        public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount, Random random)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (sampleCount <= 0)
                throw new ArgumentOutOfRangeException("sampleCount");
            if (random == null)
                throw new ArgumentNullException("random");

            return SampleCore(source, sampleCount, random);
        }

        private static IEnumerable<T> SampleCore<T>(this IEnumerable<T> source, int sampleCount, Random random)
        {
            if (!(source is IList<T> list))
            {
                list = source.ToList();
            }

            var len = list.Count;
            if (len == 0)
                yield break;

            for (var i = 0; i < sampleCount; i++)
            {
                var index = random.Next(0, len);
                yield return list[index];
            }
        }

        /// <summary>
        /// <pre>Pick up only 1 value from sequence.</pre>
        /// </summary>
        public static T SampleOne<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Sample(1).FirstOrDefault();
        }

        public static T SampleOne<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (random == null)
                throw new ArgumentNullException("random");

            return source.Sample(1, random).FirstOrDefault();
        }

        public static IEnumerable<T[]> Buffer<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            return BufferCore(source, count);
        }

        private static IEnumerable<T[]> BufferCore<T>(this IEnumerable<T> source, int count)
        {
            var buffer = new T[count];
            var index = 0;
            foreach (T item in source)
            {
                buffer[index++] = item;
                if (index == count)
                {
                    yield return buffer;
                    index = 0;
                    buffer = new T[count];
                }
            }

            if (index != 0)
            {
                var dest = new T[index];
                Array.Copy(buffer, dest, index);
                yield return dest;
            }
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentOutOfRangeException("second");

            return MergeCore(first, second);
        }

        private static IEnumerable<T> MergeCore<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            using (IEnumerator<T> e1 = first.GetEnumerator())
            using (IEnumerator<T> e2 = second.GetEnumerator())
            {
                var e2Finished = false;
                while (e1.MoveNext())
                {
                    yield return e1.Current;
                    if (!e2Finished && e2.MoveNext())
                    {
                        yield return e2.Current;
                    }
                    else
                    {
                        e2Finished = true;
                    }
                }
                while (!e2Finished && e2.MoveNext())
                {
                    yield return e2.Current;
                }
            }
        }

        /// <summary>
        /// Binary Search to sorted collection, and get nearlest value(selectLower = true:default) or upper value (selectLower = false).
        /// </summary>
        public static T BinaryRangeSearch<T, U>(this IList<T> orderedList, Func<T, U> compareSelector, U targetValue, bool selectLower = true)
            where U : IComparable<U>
        {
            if (orderedList.Count == 0)
                throw new ArgumentOutOfRangeException("You can not search for 0 element sequence.");

            var lower = -1;
            var upper = orderedList.Count;
            while (upper - lower > 1)
            {
                // == (low + high) / 2, but this avoid edge case when low+high is over upper limit.
                var index = lower + ((upper - lower) / 2);
                var compare = compareSelector(orderedList[index]).CompareTo(targetValue);
                if (compare == 0)
                {
                    lower = upper = index;
                    break;
                }
                if (compare >= 1)
                {
                    upper = index;
                }
                else
                {
                    lower = index;
                }
            }

            if (selectLower)
            {
                var selected = lower;
                if (selected <= 0)
                    selected = 0;
                return orderedList[selected];
            }
            else
            {
                var selected = upper;
                if (orderedList.Count <= selected)
                    selected = orderedList.Count - 1;
                return orderedList[selected];
            }
        }

        /// <summary>
        /// Enumerate random shuffled order Randome generator will be thread unique.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return ShuffleCore(source, RandomUtil.ThreadRandom);
        }

        /// <summary>
        /// Enumerate random shuffled order Randome generator will be use parameterd item.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return ShuffleCore(source, random);
        }

        private static IEnumerable<T> ShuffleCore<T>(this IEnumerable<T> source, Random random)
        {
            T[] buffer = source.ToArray(); // buffer for side effects

            for (var i = buffer.Length - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);

                yield return buffer[j];
                buffer[j] = buffer[i];
            }

            // return rest element
            if (buffer.Length != 0)
            {
                yield return buffer[0];
            }
        }
    }
}
