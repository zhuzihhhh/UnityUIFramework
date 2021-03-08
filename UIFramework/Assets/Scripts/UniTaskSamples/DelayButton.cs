using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DelayButton : MonoBehaviour {
    public Button btn;

    // Start is called before the first frame update
    async UniTask Start() {
        /*
         * await，会导致当前函数阻塞，由于Click是个无穷Enumerable，这里会一直阻塞，导致start不能结束。
         * await导致Start必须是async的，所以尽管Start本身被阻塞了，调用Start的caller是不会被阻塞的。
         * 如果不用await，这里不会被阻塞住，等于是在执行“注册”的过程，代码会立即执行完毕。
         */
        await btn.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async _ => {
            Debug.Log("button clicked");
            await UniTask.Delay(3000);
        });
        Debug.Log("End of start");
    }

    private void Update() {
        Debug.Log("VAR");
    }
}