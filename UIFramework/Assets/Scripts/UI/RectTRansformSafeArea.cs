using UnityEngine;

public class RectTRansformSafeArea : MonoBehaviour {
    void Start() {
        RectTransform rect = transform as RectTransform;
        var area = Screen.safeArea;
        var resolition = Screen.currentResolution;

        if (rect == null) return;
        rect.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
        rect.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
    }
}