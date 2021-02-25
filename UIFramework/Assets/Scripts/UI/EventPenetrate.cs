using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// raycast穿透组件，attach到rectTransform上，当前rectTransform上由target对象的rectTransform占据的区域的点击
/// 就能穿透到当前rectTransform的下层，主要用于新手引导的遮罩挖孔
/// 注意需要配合两个特殊的材质使用，遮罩面板必须比穿透区域占位Image的sibling值大（遮罩面板在hierarchy中处于更下方）
/// 不清楚的地方参考印象笔记中的备忘
/// </summary>
public class EventPenetrate : MonoBehaviour, ICanvasRaycastFilter {
    //作为目标点击事件渗透区域
    public Image target;

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
        //没有目标则捕捉事件渗透
        if (target == null)
            return true;
        //在目标范围内做事件渗透
        return !RectTransformUtility.RectangleContainsScreenPoint(target.rectTransform, sp, eventCamera);
    }
}