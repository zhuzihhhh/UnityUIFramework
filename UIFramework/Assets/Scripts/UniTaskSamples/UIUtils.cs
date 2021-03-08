using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour {
    public Button btn;

    private async UniTask Start() {
        int i = 0;
        AsyncMessagebroker<int> messagebroker = new AsyncMessagebroker<int>();

        messagebroker.Subscribe().Subscribe(_ => { Debug.Log($"message"); });

        this.GetAsyncUpdateTrigger().Buffer(60).Subscribe(_ => {
            i++;
            messagebroker.Publish(i);
        });

        //TODO:这个特性可以用来实现这样的需求：在按键触发的异步执行完毕之前，不再响应新的按键触发
        //在delay过程中，新“push”进来的click会被丢弃掉
        // await btn.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async x => {
        //     await UniTask.Delay(3000);
        //     Debug.Log("button clicked");
        // });

        //Queue之后，“push”进来的click不会被丢弃，会在内部的async callback结束后被依次执行
        //每个新的click进来后，还是会等待延迟，delay是从click传入异步回调开始计算，而不是从click push进来开始计算。
        // await btn.OnClickAsAsyncEnumerable().Queue().ForEachAwaitAsync(async _ => {
        //     await UniTask.Delay(3000);
        //     Debug.Log("button clicked");
        // });

        //通过Subscribe订阅的话，每个click到来之后会被立即处理，不会排队，callback中的延迟仍然有效。
        // btn.OnClickAsAsyncEnumerable().Subscribe(async _ => {
        //     await UniTask.Delay(3000);
        //     Debug.Log("button clicked");
        // });
    }

    private async UniTask _Start() {
        //CancellationTokenSource不准，对于实时生成的事件流比如update还比较好用，
        //对于用户交互触发的事件流，例如按钮点击，会有一个事件的延迟，就是在cancel之后，还能再响应一次！
        var cts = new CancellationTokenSource();

        UniTaskAsyncEnumerable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.A)).Subscribe(_ => { cts.Cancel(); });

        this.GetAsyncUpdateTrigger().Buffer(60).ForEachAsync(_ => { Debug.Log($"{"one second"}"); }, cts.Token);

        Debug.Log($"end");
        // Nonebehaviour.Reg();

        // await UniTaskAsyncEnumerable_TEST();


        await btn.OnClickAsAsyncEnumerable().ForEachAsync(_ => Debug.Log("click"), cts.Token);
    }

    //subsscribe和foreachAsync的区别很明显，
    //subscribe是一个订阅操作，表示未来会响应这个“事件”，可以看到sub函数和后面的OnClickAsync能被同一个click事件触发从概念上这里属pull，unirx是push
    //forEachAsync是个消耗操作，注意看你必须await他，这表示，在调用ForEachAsync时，他会消耗掉async队列的所有元素
    //在这之后后续的代码才会运行。
    private async Task UniTaskAsyncEnumerable_TEST() {
        // btn.OnClickAsAsyncEnumerable().Take(3).Subscribe(_ => Debug.Log("click"));
        await btn.OnClickAsAsyncEnumerable().Take(3).ForEachAsync(_ => { Debug.Log($"click:{_.ToString()}"); });
        await btn.OnClickAsync();
        Debug.Log("Button click");
        await btn.OnClickAsync();
        Debug.Log("Button click");
        await btn.OnClickAsync();
        Debug.Log("Button click");
        Debug.Log($"{"start end"}");
    }

    private void RegTrigger() {
    }
}

public static class Nonebehaviour {
    public static void Reg() {
        UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(_ => { Debug.Log($"update(){Time.frameCount}"); });
    }

    /// <summary>
    /// 得到一个新action，此action被多次执行时，被包装的action只会执行一次
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Action ExecuteOnlyOnce(Action action) {
        bool isExecuted = false;
        return () => {
            if (isExecuted) return;
            else {
                isExecuted = true;
                action?.Invoke();
            }
        };
    }
}