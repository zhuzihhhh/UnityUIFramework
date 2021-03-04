using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 排队的抽象，可以控制一个队列的起点，队伍方向，间距，最大人数限制
/// TODO:支持使用一个坐标数组来精确定位队列的每一个位置。
/// </summary>
public class GuestQueue : MonoBehaviour {
    public Vector2 QueueDirection;
    [Header("最多可排队人数")] public int Capacity;
    public float Space;
    public bool isRandomX;
    private Vector2 HeadPosition;

    private Queue<IQueueUp> queue;

    public void Awake() {
        queue = new Queue<IQueueUp>();
        HeadPosition = transform.position;
    }

    public void RemoveItem(IQueueUp item) {
        queue = new Queue<IQueueUp>(queue.Where(i => i != item));
    }

    public IQueueUp Dequeue() {
        if (queue.Count == 0) return null;

        IQueueUp dequeueObj = queue.Dequeue();

        RefreshPositions();
        dequeueObj.OnDequeue();
        return dequeueObj;
    }

    public void Clear() {
        queue.Clear();
    }

    public bool IsQueueFull() {
        return queue.Count >= Capacity;
    }

    /// <summary>
    /// 获取队列人数
    /// </summary>
    /// <returns></returns>
    public int QueueCount() {
        return queue.Count;
    }

    /// <summary>
    /// 添加排队人物
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(IQueueUp item) {
        Vector2 tailPosition = HeadPosition + Space * QueueDirection * queue.Count;
        item.SetQueuePosition(tailPosition);
        item.OnEnqueue(this);
        queue.Enqueue(item);
    }

    public IQueueUp Peek() {
        return queue.Peek();
    }

    public IQueueUp[] GetInnerQueue() {
        return queue.ToArray();
    }

    private void RefreshPositions() {
        for (int i = 0; i < queue.Count; i++) {
            queue.ElementAt(i).SetQueuePosition(HeadPosition + i * Space * QueueDirection);
        }
    }
}

public interface IQueueUp {
    void OnEnqueue(GuestQueue queue);
    void SetQueuePosition(Vector2 pos);
    bool IsInPosition(Vector2 pos);

    //void OnInPosition();
    void OnDequeue();
}