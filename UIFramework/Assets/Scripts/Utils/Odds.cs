using UnityEngine;

/// <summary>
/// 现有对象可以用这个泛型类包装自己，让其拥有一个额外的几率成员变量
/// 这个类的价值在于，可以给原本没有概率这个概念的对象增加概率的概念
/// </summary>
/// <typeparam name="T"></typeparam>
public class Odds<T> {
    public readonly T t;
    public float probability;

    public static Odds<T> Create(T t, float prob) {
        return new Odds<T>(t, prob);
    }

    private Odds(T t, float f) {
        this.t = t;
        this.probability = f;
    }

    public bool IsSuccess() {
        return Random.Range(0f, 1f) < probability;
    }
}