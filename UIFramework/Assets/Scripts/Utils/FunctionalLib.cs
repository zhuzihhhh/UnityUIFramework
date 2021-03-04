using System;

/// <summary>
/// 实现函数式的curry，以及Compose（Compose不太熟悉)
/// </summary>
public static class FunctionalLib {
    public static Func<T1, Func<T2, Tresult>> Curry<T1, T2, Tresult>(this Func<T1, T2, Tresult> func) {
        return t1 => (t2 => func(t1, t2));
    }

    public static Func<T1, Func<T2, Func<T3, Tresult>>>
        Curry<T1, T2, T3, Tresult>(this Func<T1, T2, T3, Tresult> func) {
        return t1 => t2 => t3 => func(t1, t2, t3);
    }

    public static Func<T1, Action<T2>> Curry<T1, T2>(this Action<T1, T2> action) {
        return t1 => t2 => action(t1, t2);
    }

    public static Func<T1, Func<T2, Action<T3>>> Curry<T1, T2, T3>(this Action<T1, T2, T3> action) {
        return t1 => t2 => t3 => action(t1, t2, t3);
    }

    public static Func<TSource, TEndResult> Compose<TSource, TIntermediateResult, TEndResult>(
        this Func<TSource, TIntermediateResult> func1,
        Func<TIntermediateResult, TEndResult> func2) {
        return source => func2(func1(source));
    }
}