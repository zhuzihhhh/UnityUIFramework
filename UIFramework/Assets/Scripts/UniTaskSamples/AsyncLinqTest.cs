using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class AsyncLinqTest : MonoBehaviour {
    private IDisposable disposableAsyncTask;

    private void Dead() {
        Debug.Log($"Dead, can only one time!");
    }

    // Start is called before the first frame update
    async UniTask Start() {
        // Action die = Nonebehaviour.ExecuteOnlyOnce(Dead);
        // die();
        // die();
        // die();

        // disposableAsyncTask = UniTaskAsyncEnumerable.Interval(TimeSpan.FromSeconds(1))
        //     .Subscribe(_ => { Debug.Log("Interval"); })
        //     .AddTo(this.GetCancellationTokenOnDestroy());
        //
        // await this.GetAsyncUpdateTrigger().Where(_ => Input.GetKey(KeyCode.A)).Take(1).ForEachAsync(_ => {
        //     disposableAsyncTask.Dispose();
        // });
        // Debug.Log("Start finished");


        // await this.GetAsyncUpdateTrigger().FirstAsync(_ => Input.GetKey(KeyCode.A));
        // Debug.Log("Key stroke");
        // Debug.Log($"start end");

        // 这里获得的Trigger 是一个 AsyncUpdateTrigger，继承自 AsyncTriggerBase<AsyncUnit>，
        // AsyncTriggerBase<AsyncUnit> 是一个实现 IUniTaskAsyncEnumerable<T> 的 monobehaviour
        // IuniTaskAsyncEnumerable<T> 只有一个方法
        await this.GetAsyncUpdateTrigger().Where(_ => Input.GetKeyUp(KeyCode.K)).Take(5).LastAsync();
        Debug.Log($"5 key strokes");
        Debug.Log($"start end");

        /*
         * 上面几个例子显示了unitask的命名规范，Async后缀的方法会把IEnumerable转换为一个可以被await的async
         *
         * AsyncEnumerable和unirx的Observable很类似，但是更灵活，你可以subscribe，也可以将其转化为async来await他
         * 可以通过sub的方式注册异步回调，也可以await的方式同步等待，两者的差别在于，await会让当前函数阻塞，这在有的时候会非常有用
         */


        // this.GetAsyncMouseUpAsButtonTrigger().Select(_ => Time.time)
        //     .Buffer(2, 1).Subscribe(pair => { Debug.Log($"first:{pair[0]} second:{pair[1]}"); });
    }
}