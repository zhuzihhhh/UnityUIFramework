using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeDelayUtils : MonoBehaviour {
    private void Awake() {
        Application.targetFrameRate = 60;
    }

    private void Start() {
        // Delay();
        NextFrame();
    }
    
    async UniTask Delay() {
        await UniTask.Delay(1000);
        Debug.Log("after 1000 ms");
        await UniTask.DelayFrame(60);
        Debug.Log("after 60 frames");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        Debug.Log("after TimeSpan 1 second");
    }

    async UniTask NextFrame() {
        await UniTask.WaitForEndOfFrame();
        Debug.Log("WaitforEndOfFrame");
        await UniTask.Yield();
        Debug.Log("Yield");// nextFrame和yield有区别，nextframe一定是下一帧，yield不一定是下一帧。
        await UniTask.NextFrame();
        Debug.Log("NextFrame");
        await UniTask.WaitForFixedUpdate();
        Debug.Log("WaitForFixedUpdate");
    }

    async UniTask LogicWait() {
        //等待条件成立
        await UniTask.WaitUntil(() => true);
        //等待值变化
        await UniTask.WaitUntilValueChanged(this, t=>t.transform.position);
        //等待协程结束
        await sampleRoutine();
    }

    IEnumerator sampleRoutine() {
        yield return null;
        yield return null;
    }
}