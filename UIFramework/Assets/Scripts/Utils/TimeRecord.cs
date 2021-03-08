using System;
using UnityEngine;

public struct TimeRecord {
    public bool hasRecord; //当前是否有计时器在运作
    public float TimeLeft; //剩余时间（秒）
    public DateTime TimeLeftStartAt; //剩余时间是指从什么日期时间开始的剩余时间

    public void TurnOn(float durationInSecond) {
        hasRecord = true;
        TimeLeftStartAt = DateTime.Now;
        TimeLeft = durationInSecond;
    }

    public void TurnOff() {
        hasRecord = false;
    }

    public void Update(float pass) {
        TimeLeft -= pass;
        if (TimeLeft < 0) TimeLeft = 0;
        TimeLeftStartAt = DateTime.Now;
    }

    public bool IsCompleted() {
        return TimeLeft <= 0;
    }

    /// <summary>
    /// 当你从数据库中取出一个TimeRecord时，首先要更新他经历的时间，是否完成
    /// </summary>
    public void SyncTimeRecord() {
        if (!hasRecord) return;
        var timePass = DateTime.Now - TimeLeftStartAt;
        TimeLeftStartAt = DateTime.Now;
        TimeLeft = (float) (TimeLeft - timePass.TotalSeconds);
    }

    public override string ToString() {
        return JsonUtility.ToJson(this);
    }

    public static TimeRecord FromJson(string json) {
        return JsonUtility.FromJson<TimeRecord>(json);
    }
}