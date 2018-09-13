using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ArchitectureSample.Core
{
    public static class AsyncLazy
    {
        public static AsyncLazy<T> Create<T>(Func<T> valueFactory, bool runOnTaskPool = false)
        {
            return new AsyncLazy<T>(valueFactory, runOnTaskPool);
        }

        public static AsyncLazy<T> Create<T>(Func<Task<T>> taskFactory, bool runOnTaskPool = false)
        {
            return new AsyncLazy<T>(taskFactory, runOnTaskPool);
        }

        public static AsyncLazy<T> Create<T, TState>(TState state, Func<TState, T> valueFactory, bool runOnTaskPool = true)
        {
            return new AsyncLazy<T>(() => valueFactory(state), runOnTaskPool);
        }

        public static AsyncLazy<T> Create<T, TState>(TState state, Func<TState, Task<T>> taskFactory, bool runOnTaskPool = true)
        {
            return new AsyncLazy<T>(() => taskFactory(state), runOnTaskPool);
        }

        public static AsyncLazy<T> FromResult<T>(T value)
        {
            return new AsyncLazy<T>(value);
        }

        public static Task WhenAny(params IAsyncLazy[] lazies)
        {
            return WhenAny(lazies.AsEnumerable());
        }

        public static Task WhenAny(this IEnumerable<IAsyncLazy> lazies)
        {
            return Task.WhenAny(lazies.Select(x => x.Task)).Unwrap();
        }

        public static Task WhenAll(params IAsyncLazy[] lazies)
        {
            return WhenAll(lazies.AsEnumerable());
        }

        public static Task WhenAll(this IEnumerable<IAsyncLazy> lazies)
        {
            return Task.WhenAll(lazies.Select(x => x.Task));
        }

        public static Task<T> WhenAny<T>(params AsyncLazy<T>[] lazies)
        {
            return WhenAny(lazies.AsEnumerable());
        }

        public static Task<T> WhenAny<T>(this IEnumerable<AsyncLazy<T>> lazies)
        {
            return Task.WhenAny(lazies.Select(x => x.Task)).Unwrap();
        }

        public static Task<Tuple<T1, T2>> WhenAll<T1, T2>(AsyncLazy<T1> lazy1, AsyncLazy<T2> lazy2)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2));
        }

        public static Task<Tuple<T1, T2, T3>> WhenAll<T1, T2, T3>(
            AsyncLazy<T1> lazy1,
            AsyncLazy<T2> lazy2,
            AsyncLazy<T3> lazy3)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task, lazy3.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>, AsyncLazy<T3>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value, values.Item3.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2, lazy3));
        }

        public static Task<Tuple<T1, T2, T3, T4>> WhenAll<T1, T2, T3, T4>(
            AsyncLazy<T1> lazy1,
            AsyncLazy<T2> lazy2,
            AsyncLazy<T3> lazy3,
            AsyncLazy<T4> lazy4)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task, lazy3.Task, lazy4.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>, AsyncLazy<T3>, AsyncLazy<T4>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value, values.Item3.Value, values.Item4.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2, lazy3, lazy4));
        }

        public static Task<Tuple<T1, T2, T3, T4, T5>> WhenAll<T1, T2, T3, T4, T5>(
            AsyncLazy<T1> lazy1,
            AsyncLazy<T2> lazy2,
            AsyncLazy<T3> lazy3,
            AsyncLazy<T4> lazy4,
            AsyncLazy<T5> lazy5)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task, lazy3.Task, lazy4.Task, lazy5.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>, AsyncLazy<T3>, AsyncLazy<T4>, AsyncLazy<T5>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value, values.Item3.Value, values.Item4.Value, values.Item5.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2, lazy3, lazy4, lazy5));
        }

        public static Task<Tuple<T1, T2, T3, T4, T5, T6>> WhenAll<T1, T2, T3, T4, T5, T6>(
            AsyncLazy<T1> lazy1,
            AsyncLazy<T2> lazy2,
            AsyncLazy<T3> lazy3,
            AsyncLazy<T4> lazy4,
            AsyncLazy<T5> lazy5,
            AsyncLazy<T6> lazy6)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task, lazy3.Task, lazy4.Task, lazy5.Task, lazy6.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>, AsyncLazy<T3>, AsyncLazy<T4>, AsyncLazy<T5>, AsyncLazy<T6>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value, values.Item3.Value, values.Item4.Value, values.Item5.Value, values.Item6.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2, lazy3, lazy4, lazy5, lazy6));
        }

        public static Task<Tuple<T1, T2, T3, T4, T5, T6, T7>> WhenAll<T1, T2, T3, T4, T5, T6, T7>(
            AsyncLazy<T1> lazy1,
            AsyncLazy<T2> lazy2,
            AsyncLazy<T3> lazy3,
            AsyncLazy<T4> lazy4,
            AsyncLazy<T5> lazy5,
            AsyncLazy<T6> lazy6,
            AsyncLazy<T7> lazy7)
        {
            return Task.WhenAll(lazy1.Task, lazy2.Task, lazy3.Task, lazy4.Task, lazy5.Task, lazy6.Task, lazy7.Task).ContinueWith((_, state) =>
            {
                var values = (Tuple<AsyncLazy<T1>, AsyncLazy<T2>, AsyncLazy<T3>, AsyncLazy<T4>, AsyncLazy<T5>, AsyncLazy<T6>, AsyncLazy<T7>>)state;
#pragma warning disable 618
                return Tuple.Create(values.Item1.Value, values.Item2.Value, values.Item3.Value, values.Item4.Value, values.Item5.Value, values.Item6.Value, values.Item7.Value);
#pragma warning restore 618
            }, Tuple.Create(lazy1, lazy2, lazy3, lazy4, lazy5, lazy6, lazy7));
        }

        public static Task<T[]> WhenAll<T>(this IEnumerable<AsyncLazy<T>> lazies)
        {
            return Task.WhenAll(lazies.Select(x => x.Task).ToArray());
        }

        public static void WaitAll(params IAsyncLazy[] lazies)
        {
            WaitAll(lazies.AsEnumerable());
        }

        public static void WaitAll(this IEnumerable<IAsyncLazy> lazies)
        {
            Task.WaitAll(lazies.Select(x => x.Task).ToArray());
        }

        public static int WaitAny(params IAsyncLazy[] lazies)
        {
            return WaitAny(lazies.AsEnumerable());
        }

        public static int WaitAny(this IEnumerable<IAsyncLazy> lazies)
        {
            return Task.WaitAny(lazies.Select(x => x.Task).ToArray());
        }

        public static AsyncLazy<TResult> Select<T, TResult>(this AsyncLazy<T> lazy, Func<T, TResult> selector, bool configureAwait = false)
        {
            return new AsyncLazy<TResult>(async () => selector(await lazy.Task.ConfigureAwait(configureAwait)));
        }

        public static AsyncLazy<TResult> Select<T, TResult>(this AsyncLazy<T> lazy, Func<T, Task<TResult>> selector, bool configureAwait = false)
        {
            return new AsyncLazy<TResult>(async () => await selector(await lazy.Task.ConfigureAwait(configureAwait)));
        }
    }

    public interface IAsyncLazy
    {
        bool IsValueCreated { get; }
        Task Task { get; }
    }

    [DebuggerDisplay("{ValueForDebugDisplay}")]
    public class AsyncLazy<T> : IAsyncLazy
    {
        readonly Lazy<Task<T>> lazyValue;

        internal AsyncLazy(T value)
        {
            lazyValue = new Lazy<Task<T>>(() => System.Threading.Tasks.Task.FromResult(value), isThreadSafe: false);
            Start();
        }

        public AsyncLazy(Func<T> valueFactory, bool runOnTaskPool = false)
        {
            lazyValue = runOnTaskPool
                ? new Lazy<Task<T>>(() => System.Threading.Tasks.Task.Run(valueFactory))
                : new Lazy<Task<T>>(() => System.Threading.Tasks.Task.FromResult<T>(valueFactory()));
        }

        public AsyncLazy(Func<Task<T>> taskFactory, bool runOnTaskPool = false)
        {
            lazyValue = runOnTaskPool
                ? new Lazy<Task<T>>(() => System.Threading.Tasks.Task.Run(taskFactory))
                : new Lazy<Task<T>>(taskFactory);
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return lazyValue.Value.GetAwaiter();
        }

        public void Start()
        {
            var _ = lazyValue.Value; // Task Start
        }

        Task IAsyncLazy.Task => lazyValue.Value;

        /// <summary>
        /// valueFactoryの実行を開始しTaskで待ちます。
        /// </summary>
        public Task<T> Task => lazyValue.Value;

        /// <summary>
        /// 同期的に値を取得します。同期的な取得の前に、まずはAsyncLazyそのものをawaitすることを検討してください。
        /// </summary>
#if !LEGACY_CORE
        [Obsolete(".Valueは原則避けてawaitしてください。")]
#endif
        public T Value => lazyValue.Value.Result;

        public bool IsValueCreated => lazyValue.IsValueCreated;

        private string ValueForDebugDisplay
        {
            get
            {
                if (IsValueCreated)
                {
                    var task = lazyValue.Value;
                    if (task.IsCompleted)
                    {
                        return task.Result.ToString();
                    }
                    else
                    {
                        return "TaskStatus:" + task.Status;
                    }
                }
                else
                {
                    return "Value is not created yet.";
                }
            }
        }
    }
}
