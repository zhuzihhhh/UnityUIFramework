using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 集合类扩展方法：
/// </summary>
public static class CollectionExtensions {

    private static readonly System.Random rand = new System.Random();
    /// <summary>
    /// 扩展Array的方法，让Array拥有洗牌功能 in-place Shuffle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(this T[] array) {
        int currentIndex;
        T tempValue;
        for (int i = array.Length - 1; i >= 0; i--) {
            currentIndex = UnityEngine.Random.Range(0, i + 1);
            tempValue = array[currentIndex];
            array[currentIndex] = array[i];
            array[i] = tempValue;
        }
    }
    /// <summary>
    /// 扩展List的方法，List<T>可以就地洗牌 in-place Shuffle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list) {
        int currentIndex;
        T tempValue;
        for (int i = list.Count - 1; i >= 0; i--) {
            currentIndex = UnityEngine.Random.Range(0, i + 1);
            tempValue = list[currentIndex];
            list[currentIndex] = list[i];
            list[i] = tempValue;
        }
    }

    public static T Random<T>(this T[] array) {
        if (array == null || array.Length == 0) return default(T);
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T Random<T>(this List<T> array) {
        if (array == null || array.Count == 0) return default(T);
        return array[UnityEngine.Random.Range(0, array.Count)];
    }


    /// <summary>
    /// 扩展Dictionary，用value反向查key，注意value必须是object子类
    /// </summary>
    /// <typeparam name="Tkey">key的类型</typeparam>
    /// <typeparam name="Tvalue">value的类型</typeparam>
    /// <param name="dict">字典本身</param>
    /// <param name="value">用来查找的值</param>
    /// <returns>如果找到了，返回key，否则返回default，通常至少会用string做key，所以key假定是reference，不是value</returns>
    public static Tkey GetKeyByVaule<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tvalue value) {
        if (dict.ContainsValue(value)) {
            var kvPair = dict.ToList().Find(kv => kv.Value.Equals(value));
            return kvPair.Key;
        }
        return default;
    }

    public static Tkey RandomKey<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict) {
        return dict.Keys.ElementAt(rand.Next(0, dict.Keys.Count));
    }

    public static Tvalue RandomValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict) {
        return dict.Values.ElementAt(rand.Next(0, dict.Values.Count));
    }


    public static KeyValuePair<Tkey, Tvalue> RandomKeyValuePair<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict) {
        return dict.ElementAt(rand.Next(0, dict.Count));
    }

    /// <summary>
    /// 将queue中任意位置的一个元素删除，queue默认只能删除队首和队尾的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queue"></param>
    /// <param name="element"></param>
    public static void RemoveFromQueue<T>(this Queue<T> queue, T element) {
        queue = new Queue<T>(queue.Where(i => !i.Equals(element)));
    }
}
