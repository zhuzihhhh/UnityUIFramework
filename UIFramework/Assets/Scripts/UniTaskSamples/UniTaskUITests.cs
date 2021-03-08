using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;

public class UniTaskUITests : MonoBehaviour {
    public Button okButton;
    public Transform tweenObj;

    private void Start() {
        // 按键一旦触发后，2秒之内不再响应

        CancellationTokenSource cts = new CancellationTokenSource();

        RegisterButton(cts.Token);

        cts.Cancel();
    }

    /// <summary>
    /// register a button click handler callback, 
    /// the callback cannot be triggered until N milliseconds after last time triggered
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="token"></param>
    /// <param name="action"></param>
    /// <param name="throttle"></param>
    async void RegisterbuttonThrottle(Button btn, CancellationToken token, Action action, int throttle) {
        while (!token.IsCancellationRequested) {
            await btn.OnClickAsync();
            action?.Invoke();
            await UniTask.Delay(throttle);
        }
    }
    async void RegisterButton(CancellationToken token) {
        while (true && !token.IsCancellationRequested) {
            Debug.Log("等待按下按钮");
            await okButton.OnClickAsync();
            //处理按钮按下的callback
            Debug.Log("按下了按钮");

            //显示进度环或者其他提示，此时不再响应按钮按下。
            Debug.Log("现在不会响应");
            await UniTask.Delay(4000);
        }
    }


    /// <summary>
    /// ForEachAsync 和 ForEachAwaitAsync 的区别，关键在于Await上
    /// ForEachAsync字面意思是异步遍历（一个事件队列）本质上就是subscribe一个事件流。需要传入一个同步Action
    /// ForEachAwaitAsync 等待每个异步事件到来后执行一个异步子任务,当子任务完成后才会接受下一个异步事件（如果提前到来要排队？）
    /// 下面例子验证，使用Await版本，Upate被延迟到两秒响应一次输出日志。
    /// </summary>
    async void Compare_ForEachAsync_ForEachAwaitAsync() {

        //这个方法可以不await，编译器会提示，但是你可以不等待，这里就是个例子，如果你await一个无限的事件源，
        //那么await之后的语句就无法被执行了，下面的second await就没机会被注册到
        this.GetAsyncUpdateTrigger().ForEachAwaitAsync(async _ => {
            Debug.Log("before await");
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Debug.Log("after await");
        }).Forget();    //Forget让你不需要await（如果你直接不await的话，编译器会警告你，但不会编译失败，这个相当于显式告诉编译器，这里我不想等他完成，
        //我只是在这里声明一个响应callback，请继续执行下面的代码，如果你await的话，会等待当前的task完成，但是Update，或者无终止条件的ui交互事件是
        //无法结束的，会导致await之后的代码永远无法执行，尽管后面的代码实际也只是在注册回调，但这就是UniTask的特点，等待task完成，和监听一个task的Enumerable
        //是统一的。
        //如何理解uniTask？它实际上统一了等待N次发生和注册无限次发生的事件流的回调这两种模式，这是比Rx更加先进和简单的地方
        //你需要熟悉await这种模式的行为细节。习惯之后它确实比rx更好用，但rx的概念在别的语言环境中也普遍存在，但async-await主要存在于.net中。
        await this.GetAsyncUpdateTrigger().ForEachAsync(_ => Debug.Log("second await"));
    }

    public void AsyncReactivePropertyTest() {
        var rp = new AsyncReactiveProperty<int>(1000);

        //Forget让你对异步直接注册一个回调，不需要再await他了
        rp.ForEachAsync<int>(i => Debug.Log($"{i}")).Forget();

        rp.Value = 11;
        rp.Value = 22;
    }

    // Start is called before the first frame update
    async void start_old() {
        //FireAndForgetMethod().Forget();
        //AsyncReactivePropertyTest();
        Compare_ForEachAsync_ForEachAwaitAsync();

        //await UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(_ => Debug.Log("static update...."));

        //uniTask原生支持DoTween，很实用，注意Dotween必须使用openupm安装，或者需要指定一个预处理选项
        //
        // sequential
        //await tweenObj.DOMoveX(2, 5);
        //Debug.Log(" tween");
        //await tweenObj.DOMoveY(2, 5);
        //Debug.Log(" tween");

        //// parallel with cancellation
        //var ct = this.GetCancellationTokenOnDestroy();

        //await UniTask.WhenAll(
        //    tweenObj.DOMoveX(-2, 3).WithCancellation(ct),
        //    tweenObj.DOScale(3, 3).WithCancellation(ct));
        //Debug.Log(" tween");

        //注册一个callback，每次点击都call
        //okButton.onClick.AddListener(() => {
        //    Debug.Log("常规按键响应");
        //});

        //只等待一次点击发生，当前函数将一直阻塞到按键发生，但是caller不会被阻塞
        //await okButton.OnClickAsync();
        //Debug.Log("button Click");

        //AsyncEnumerable类似Observable
        //await okButton.OnClickAsAsyncEnumerable().Where((x, i) => i % 2 == 0).
        //    ForEachAsync(_ => {
        //    Debug.Log("every to click button");
        //});


        //Observable的缺点就是太难理解了，如果对Reactive编程不是很熟悉，Rx的代码是很难理解的

        //可以同步的方式编写异步代码，等待5秒，然后才启用点击callback
        //这里体现出uniTask的优势，如果用Rx实现这里的效果还是麻烦很多，需要OnClickAsObservable().Delay(Timespan.FromSeconds(5))
        //如果你熟悉Rx，感觉并没有麻烦很多，但UniTask让代码更加贴近人的思维模式
        //await UniTask.Delay(TimeSpan.FromSeconds(5), false);//默认延迟受timescale影响。

        //Debug.Log("after 5 seconds");

        ////asyncEnumerable可以像observable一样subcribe
        //okButton.OnClickAsAsyncEnumerable().Where((x, i) => i % 2 == 0)
        //    .Subscribe(_ => flag = true); ;

        //var cts = new CancellationTokenSource();//.Token;
        //cts.Cancel();

        ////等待一个指定的条件满足，看起来很简单，其实功能超强
        // WithCancellation可以传入一个CancellationToken，这样你可以在需要的情况下cancel这个uniTask
        // 在task内部要退出的话都是靠抛异常，但是如果你捕获异常会导致性能问题，只要你确定不会有别的问题，可以SuppressCancellationThrow来无声的忽略此异常。
        //await UniTask.WaitUntilValueChanged(this, self => self.flag == true).WithCancellation(cts.Token).SuppressCancellationThrow();
        //Debug.Log("wait until flag is true");

        ////等待一个coroutine完成，看起来很简单，其实功能超强
        //await getRoutine();
        //Debug.Log("coroutine finished");

        //AsyncMessageBroker<string> amb = new AsyncMessageBroker<string>();
        //amb.Subscribe().Subscribe(msg => Debug.Log($"{msg}"));
        //amb.Subscribe().Subscribe(msg => Debug.Log($"another {msg} text"));

        ////注册一个callback，每次点击都call
        //okButton.onClick.AddListener(() => {
        //    amb.Publish("pressed");
        //});

        Debug.Log("proof of start not blocked");

    }

    private bool flag = false;

    IEnumerator getRoutine() {
        yield return new WaitForSeconds(2);
        Debug.Log("coroutine wait 2sec");
        yield return new WaitForSeconds(2);
        Debug.Log("coroutine wait 2sec");
        yield return new WaitForSeconds(2);
        Debug.Log("coroutine wait 2sec");
    }

    /// <summary>
    /// 在C#7.0中创建自己的UniTaskAsyncEnumerable,
    /// C#8.0有更加简洁的语法支持，但在使用Unity2020之前不考虑它。
    /// 
    /// 除非真的很通用，你可能没有必要实现自己的UniTaskAsyncEnumerable
    /// 通常特殊条件才yield产出的Enumerable都可以通过现有的AsyncEnumerable过滤来获得。
    /// </summary>
    /// <returns></returns>
    public IUniTaskAsyncEnumerable<int> MyEveryUpdate() {
        return UniTaskAsyncEnumerable.Create<int>(async (writer, token) => {
            var frameCount = 0;
            await UniTask.Yield();
            while (!token.IsCancellationRequested) {
                await writer.YieldAsync(frameCount++);
                await UniTask.Yield();
            }
        });
    }


    /// <summary>
    /// 把回调模式转变成一个UniTask
    /// </summary>
    /// <returns></returns>
    public UniTask<int> ConvertEventCallBackToUniTask() {
        var utcs = new UniTaskCompletionSource<int>();
        //register your callback to a event, callback should call TrySetResult/TrySetCancel/TrySetException when appropriate
        utcs.TrySetResult(10);
        return utcs.Task;
    }

    /// <summary>
    /// 每个UniTask都可以转化为Coroutine，用在必须使用coroutine的场合。
    /// </summary>
    /// <returns></returns>
    public IEnumerator ToCoroutine() {
        return ConvertEventCallBackToUniTask().ToCoroutine();
    }

    public async UniTaskVoid FireAndForgetMethod() {
        // do anything...
        await UniTask.Yield();
    }

}


//UniTask提供了更加基础的Channel，由channel可以包装出一个MessageBroker
//如果不需要一个生产者对应多个消费者，就可以直接使用Channel
//具体的信道，订阅，发送暴露出更多的低层细节给你，不像Rx直接提供一个高级抽象的中介出来
//UniRx中的MessageBroker也有其不足。
public class AsyncMessageBroker<T> : IDisposable {
    //channel是个多个生产者对应一个消费者的信道
    Channel<T> channel;

    IConnectableUniTaskAsyncEnumerable<T> multicastSource;
    IDisposable connection;

    //这里使用了Rx的一个技巧，让一个消费者订阅Channel，然后把这个消费者作为新的广播者，就可以被多个消费订阅了。
    public AsyncMessageBroker() {
        channel = Channel.CreateSingleConsumerUnbounded<T>();
        multicastSource = channel.Reader.ReadAllAsync().Publish();
        connection = multicastSource.Connect(); // Publish returns IConnectableUniTaskAsyncEnumerable.
    }

    public void Publish(T value) {
        channel.Writer.TryWrite(value);
    }

    public IUniTaskAsyncEnumerable<T> Subscribe() {
        return multicastSource;
    }

    public void Dispose() {
        channel.Writer.TryComplete();
        connection.Dispose();
    }
}
