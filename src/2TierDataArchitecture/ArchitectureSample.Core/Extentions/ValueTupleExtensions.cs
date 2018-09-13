﻿using System.Threading.Tasks;

namespace ArchitectureSample.Core
{
    public static class ValueTupleExtensions
    {
        public static async Task<(T1, T2)> WhenAll<T1, T2>(this (Task<T1>, Task<T2>) tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(false);
            return (tasks.Item1.Result, tasks.Item2.Result);
        }

        public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(false);
            return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
        }

        public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(false);
            return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result);
        }
    }
}
