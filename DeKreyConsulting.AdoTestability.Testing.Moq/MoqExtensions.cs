using Moq.Language;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moq
{
    public static class MoqExtensions
    {
        public static IReturnsResult<TMock> ReturnsWithDelay<TMock>(this IReturns<TMock, Task> input)
            where TMock : class
        {
            return input.Returns(() => Task.Delay(10));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult>(this IReturns<TMock, Task<TResult>> input, TResult value)
            where TMock : class
        {
            return input.Returns(() => Task.Delay(10).ContinueWith(t => value));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>((a, b, c, d, e, f, g, h, i, j, k, l, m, n) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k, l, m, n)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>((a, b, c, d, e, f, g, h, i, j, k, l, m) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k, l, m)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((a, b, c, d, e, f, g, h, i, j, k, l) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k, l)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((a, b, c, d, e, f, g, h, i, j, k) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j, k)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>((a, b, c, d, e, f, g, h, i, j) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i, j)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>((a, b, c, d, e, f, g, h, i) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h, i)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7, T8>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7, T8>((a, b, c, d, e, f, g, h) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g, h)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6, T7>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6, T7>((a, b, c, d, e, f, g) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f, g)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5, T6>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5, T6>((a, b, c, d, e, f) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e, f)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4, T5>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, T5, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4, T5>((a, b, c, d, e) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d, e)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3, T4>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, T4, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3, T4>((a, b, c, d) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c, d)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2, T3>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, T3, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2, T3>((a, b, c) => Task.Delay(10).ContinueWith(t => valueFunction(a, b, c)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1, T2>(this IReturns<TMock, Task<TResult>> input, Func<T1, T2, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1, T2>((a, b) => Task.Delay(10).ContinueWith(t => valueFunction(a, b)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult, T1>(this IReturns<TMock, Task<TResult>> input, Func<T1, TResult> valueFunction)
            where TMock : class
        {
            return input.Returns<T1>((a) => Task.Delay(10).ContinueWith(t => valueFunction(a)));
        }

        public static IReturnsResult<TMock> ReturnsWithDelay<TMock, TResult>(this IReturns<TMock, Task<TResult>> input, Func<TResult> valueFunction)
            where TMock : class
        {
            return input.Returns(() => Task.Delay(10).ContinueWith(t => valueFunction()));
        }
    }
}
