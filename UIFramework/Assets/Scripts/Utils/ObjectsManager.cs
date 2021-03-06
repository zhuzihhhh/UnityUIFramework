using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectsManager<T> : MonoBehaviour, IEnumerable<T> where T : MonoBehaviour {
    public T[] prefabArray;
    private List<T> goList = new List<T>();

    public IEnumerator<T> GetEnumerator() {
        return goList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <summary>
    /// 用prefabArray的第一个prefab创建实例
    /// </summary>
    /// <returns></returns>
    public T Create() {
        var go = Instantiate(prefabArray[0], transform);
        goList.Add(go);
        return go;
    }

    /// <summary>
    /// 用prefabArray中的随机一个prefab创建实例
    /// </summary>
    /// <returns></returns>
    public T CreateRandom() {
        var go = Instantiate(prefabArray[Random.Range(0, prefabArray.Length)], transform);
        goList.Add(go);
        return go;
    }

    /// <summary>
    /// 用prefabArray中的第一个满足predicate的prefab创建实例
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T Create(Func<T, bool> predicate) {
        try {
            var prefab = prefabArray.First(predicate);
            var go = Instantiate(prefab, transform);
            goList.Add(go);
            return go;
        }
        catch (Exception e) {
            Debug.LogError($"{e.ToString()}");
            return default(T);
        }
    }

    public bool Delete(T t) {
        if (goList.Contains(t)) {
            goList.Remove(t);
            Destroy(t.gameObject);
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// 所有LINQ操作符都可以使用，没有必要专门实现find，filter，map了
    /// 可以在派生类中根据业务逻辑封装专门的接口
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
}