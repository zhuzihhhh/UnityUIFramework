using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TileMarkGroup : MonoBehaviour, IEnumerable<TileMark> {
    [SerializeField] private TileMark tileMarkPrefab;
    private List<TileMark> marks;


    public int Row;
    public int Col;

    public Vector3 DragStartPostion { get; set; }

    // Start is called before the first frame update
    void Start() {
        marks = new List<TileMark>(GetComponentsInChildren<TileMark>());
        foreach (var tleMark in marks) {
            tleMark.group = this;
        }

        transform.position = SavedPosition;
        //初始化，根据当前位置
        Refresh();
        UpdateGroupTilesUseage();
        TileMap.Instance.ResetAllColor();
    }

    [ExecuteInEditMode]
    [ContextMenu("创建TileMarkGroup")]
    public void CreateTileMarkGroup() {
        for (int i = 0; i < Row; i++) {
            for (int j = 0; j < Col; j++) {
                var mark = Instantiate(tileMarkPrefab, transform);
                var size = mark.GetComponent<SpriteRenderer>().bounds.size;
                mark.transform.localPosition =
                    new Vector3(size.x * j - (Col - 1) * size.x / 2, size.y * i - (Row - 1) * size.y / 2, -1);
            }
        }
    }

    [ExecuteInEditMode]
    [ContextMenu("删除TileMarkGroup")]
    public void DeleteTileMarkGroup() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    [ExecuteInEditMode]
    [ContextMenu("应用所有tileMarkGroup的保存位置")]
    public void ApplySavedPosition() {
        var allGroup = GameObject.FindObjectsOfType<TileMarkGroup>();
        foreach (var tileMarkGroup in allGroup) {
            tileMarkGroup.transform.position = tileMarkGroup.SavedPosition;
        }
    }

    [ExecuteInEditMode]
    [ContextMenu("删除所有tileMarkGroup的保存位置")]
    public void DeleteAllSavedPosition() {
        var allGroup = GameObject.FindObjectsOfType<TileMarkGroup>();
        foreach (var group in allGroup) {
            group.DeleteSavedPosition();
        }
    }

    private Vector3 SavedPosition {
        get
        {
            var v3Str = PlayerPrefs.GetString(gameObject.name, "");
            if (string.IsNullOrEmpty(v3Str)) return transform.position;
            else return JsonUtility.FromJson<Vector3>(v3Str);
        }
        set { PlayerPrefs.SetString(gameObject.name, JsonUtility.ToJson(transform.position)); }
    }

    private void DeleteSavedPosition() {
        PlayerPrefs.DeleteKey(gameObject.name);
    }

    public void Refresh() {
        foreach (var i in TileMap.Instance) {
            i.SetColor(TileMap.Instance.defaultColor);
        }

        marks.ForEach(m => {
            var collider2Ds = Physics2D.OverlapPointAll(m.transform.position);

            var overlapTiles = collider2Ds
                .Where(col => col != m.tileMarkCollider2D)
                .Select(col => col.GetComponent<Tile>())
                .Where(tile => tile != null);

            if (overlapTiles.Count() == 0) {
                m.OverTile = null;
            }
            else {
                m.OverTile = overlapTiles.First();
                if (IsMarkOnLegalTile(m))
                    m.OverTile.SetColor(TileMap.Instance.legalColor);
                else
                    m.OverTile.SetColor(TileMap.Instance.illegalColor);
            }
        });
    }

    public void UpdateGroupTilesUseage() {
        if (IsAllMarkOnLegalTile()) {
            var offset = (Vector2) marks[0].OverTile.transform.position - (Vector2) marks[0].transform.position;

            transform.DOMove(offset, 0.2f).SetRelative().OnComplete(() => { SavedPosition = transform.position; });
            ConfirmTileUse();
        }
        else {
            transform.DOMove(DragStartPostion, 0.3f);
        }
    }

    /// <summary>
    /// 判断TileMark是否处在一个合法的Tile上方,对于一个mark来说，合法的tile必须满足一下条件
    /// tile在mark下方；tile没有被占用 或者 tile被当前markgroup所在的mark占用
    /// </summary>
    /// <param name="tileMark"></param>
    /// <returns></returns>
    private bool IsMarkOnLegalTile(TileMark tileMark) {
        return tileMark.OverTile && (!tileMark.OverTile.IsUsed || IsTileInGroupUsedTiles(tileMark.OverTile));
    }

    /// <summary>
    /// 判断Group的所有TileMark是否都处在合法的Tile上方，这样才能落位
    /// </summary>
    /// <returns></returns>
    private bool IsAllMarkOnLegalTile() {
        return marks.All(mk => IsMarkOnLegalTile(mk));
    }

    private void ConfirmTileUse() {
        marks.ForEach(mk => {
            mk.UsedTile?.UnUse();
            mk.OverTile.Use(mk);
        });
    }


    /// <summary>
    /// 测试给定的tile t 是否已经被group的某个mark占用
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private bool IsTileInGroupUsedTiles(Tile t) {
        try {
            return marks.Select(m => m.UsedTile).Contains(t);
        }
        catch (Exception e) {
            return false;
        }
    }

    public IEnumerator<TileMark> GetEnumerator() {
        return marks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}