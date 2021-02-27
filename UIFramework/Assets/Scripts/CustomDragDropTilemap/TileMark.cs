using UnityEngine;

public class TileMark : MonoBehaviour {
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public BoxCollider2D collider2D;
    public Tile OverTile { get; set; }
    public Tile UsedTile { get; set; }

    public TileMarkGroup group { get; set; }

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();
        var pos = transform.position;
        pos.z = -1;
        transform.position = pos;
    }

    private void OnMouseDown() {
        group.DragStartPostion = group.transform.position;
    }

    private void OnMouseDrag() {
        //当前鼠标位置与初始鼠标点下位置的偏移量
        var offset = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset.z = -1;

        group.transform.position = offset;
        group.Refresh();
    }

    private void OnMouseUp() {
        Debug.Log("Mouse Up");
        TileMap.Instance.ResetAllColor();

        group.UpdateGroupTilesUseage();
    }
}