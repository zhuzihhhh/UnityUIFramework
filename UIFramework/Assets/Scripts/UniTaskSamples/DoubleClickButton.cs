using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class DoubleClickButton : MonoBehaviour {
    public Button btn;

    // 双击的实现，unirx中使用了TimeInterval，发现unitask并不支持，但其实可以自己实现一个
    // Review Buffer的参数，skip：前一个buffer开始的位置到下一个buffer开始位置
    void Start() {
        RegisterHoldPress(() => { Debug.Log("keep pressed trigger!"); });
        btn.GetAsyncPointerClickTrigger().Subscribe(_ => Debug.Log("clicked"));
    }

    /// <summary>
    /// 按下之后开启一个定时器，定期触发，当松开之后删除这个定时器
    /// </summary>
    /// <param name="action"></param>
    void RegisterHoldPress(Action action) {
        btn.GetAsyncPointerDownTrigger()
            .Subscribe(
                async _ => {
                    UniTaskAsyncEnumerable.Timer(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(250))
                        .TakeUntil(btn.GetAsyncPointerUpTrigger().FirstAsync()).Subscribe(o => { action.Invoke(); })
                        .AddTo(this.GetCancellationTokenOnDestroy());
                }
            );
    }

    void RegisterDoubleClick(Action action) {
        btn.GetAsyncPointerClickTrigger().Select(_ => Time.time)
            .Buffer(2, 1) //.Subscribe(pair => { Debug.Log($"first:{pair[0]} second:{pair[1]}"); });
            .Select(pair => pair[1] - pair[0])
            .Where(interval => interval < 0.25f)
            .Subscribe(_ => {
                Debug.Log("Double clicked");
                action.Invoke();
            });
    }
}