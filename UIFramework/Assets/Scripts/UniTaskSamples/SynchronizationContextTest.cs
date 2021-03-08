using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class SynchronizationContextTest : MonoBehaviour {
    private static SynchronizationContext context;

    // Start is called before the first frame update
    void Start() {
        context = new SynchronizationContext();
        Debug.Log($"main thread id:{Thread.CurrentThread.ManagedThreadId}" );
        
        var thread = new Thread(ThreadMethod);
        thread.Start();
        
        Thread.Sleep(6000);
        Debug.Log("main thread uses synchronizationContext:");
        context.Send(EventMethod, "main thread Send");
        context.Post(EventMethod,"main thread post");
        Debug.Log("main thread finish");
    }

    static void ThreadMethod() {
        Debug.Log($"child thread id: ${Thread.CurrentThread.ManagedThreadId}");
        context.Send(EventMethod,"child thread Send");
        context.Post(EventMethod, "child thread post");
        Debug.Log($"child thread finish");
    } 

    static void EventMethod(object arg) {
        Debug.Log($"EventMethod call type: {(string)arg} runing on:{Thread.CurrentThread.ManagedThreadId}");
    }
}